using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityCore
{
    /// <summary>
    /// Main-thread audio playback. The latest request owns each looping channel.
    /// Pending and playing one-shots share a bounded pool and new requests are dropped when it is full.
    /// Each playback task completes when it starts the sound or skips a superseded request.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AudioService : MonoBehaviour
    {
        private const string MusicEnabledKey = "audio.music.enabled";
        private const string SfxEnabledKey = "audio.sfx.enabled";
        private const string MusicVolumeKey = "audio.music.volume";
        private const string SfxVolumeKey = "audio.sfx.volume";

        [SerializeField, Min(1)] private int sfxCapacity = 8;

        private readonly Channel music = new Channel();
        private readonly Channel loop = new Channel();
        private readonly List<Channel> effects = new List<Channel>();
        private ResourceStore resources;
        private LocalStore settings;
        private GameObject sourceHost;
        private bool shutdown;
        private bool musicEnabled = true;
        private bool sfxEnabled = true;
        private float musicVolume = 1f;
        private float sfxVolume = 1f;

        public bool MusicEnabled
        {
            get => musicEnabled;
            set
            {
                if (musicEnabled == value) return;
                musicEnabled = value;
                if (!value) StopMusic();
                settings?.SetBool(MusicEnabledKey, value);
                settings?.Save();
            }
        }

        public bool SfxEnabled
        {
            get => sfxEnabled;
            set
            {
                if (sfxEnabled == value) return;
                sfxEnabled = value;
                if (!value) StopEffects();
                settings?.SetBool(SfxEnabledKey, value);
                settings?.Save();
            }
        }

        public float MusicVolume
        {
            get => musicVolume;
            set
            {
                value = NormalizeVolume(value);
                if (musicVolume == value) return;
                musicVolume = value;
                music.SetVolume(value);
                settings?.SetFloat(MusicVolumeKey, value);
                settings?.Save();
            }
        }

        public float SfxVolume
        {
            get => sfxVolume;
            set
            {
                value = NormalizeVolume(value);
                if (sfxVolume == value) return;
                sfxVolume = value;
                loop.SetVolume(value);
                foreach (var effect in effects)
                    effect.SetVolume(value);
                settings?.SetFloat(SfxVolumeKey, value);
                settings?.Save();
            }
        }

        public void Initialize(ResourceStore resourceStore, LocalStore localStore)
        {
            if (shutdown) throw new ObjectDisposedException(nameof(AudioService));
            if (resources != null) throw new InvalidOperationException("AudioService is already initialized.");
            if (resourceStore == null) throw new ArgumentNullException(nameof(resourceStore));
            if (localStore == null) throw new ArgumentNullException(nameof(localStore));

            musicEnabled = localStore.GetBool(MusicEnabledKey, true);
            sfxEnabled = localStore.GetBool(SfxEnabledKey, true);
            musicVolume = NormalizeVolume(localStore.GetFloat(MusicVolumeKey, 1f));
            sfxVolume = NormalizeVolume(localStore.GetFloat(SfxVolumeKey, 1f));
            resources = resourceStore;
            settings = localStore;
        }

        public Task PlayMusicAsync(string address) => PlayAsync(address, music, true, 1f);

        public Task PlayLoopAsync(string address) => PlayAsync(address, loop, true, 1f);

        public Task PlaySfxAsync(string address, float pitch = 1f)
        {
            if (!CanPlay(false)) return Task.CompletedTask;

            var channel = GetAvailableEffect();
            return channel == null ? Task.CompletedTask : PlayAsync(address, channel, false, pitch);
        }

        public void StopMusic() => music.Stop();

        public void StopLoop() => loop.Stop();

        /// <summary>Stops playback and releases its leases before the owner disposes the resource store.</summary>
        public void Shutdown()
        {
            if (shutdown) return;
            shutdown = true;
            StopMusic();
            StopEffects();
            resources = null;
            settings = null;

            if (sourceHost == null) return;
            Destroy(sourceHost);
            sourceHost = null;
        }

        private async Task PlayAsync(string address, Channel channel, bool shouldLoop, float pitch)
        {
            var isMusic = channel == music;
            if (!CanPlay(isMusic)) return;
            if (resources == null) throw new InvalidOperationException("Initialize AudioService before playing audio.");

            var request = ++channel.Request;
            channel.Pending = true;
            AssetLease<AudioClip> lease = null;
            try
            {
                lease = await resources.AcquireAsync<AudioClip>(address);
                if (channel.Request != request) return;

                var clip = lease.Asset;
                channel.Clear();
                if (channel.Source == null) channel.Source = CreateSource(shouldLoop);
                channel.Source.clip = clip;
                channel.Source.pitch = pitch;
                channel.SetVolume(isMusic ? musicVolume : sfxVolume);
                channel.Source.Play();
                channel.Lease = lease;
                lease = null;
            }
            catch (ObjectDisposedException) when (channel.Request != request)
            {
                // The owner can dispose resources while an invalidated request is still loading.
            }
            finally
            {
                if (channel.Request == request) channel.Pending = false;
                lease?.Dispose();
            }
        }

        private bool CanPlay(bool isMusic) =>
            !shutdown && isActiveAndEnabled && (isMusic ? musicEnabled : sfxEnabled);

        private Channel GetAvailableEffect()
        {
            foreach (var channel in effects)
                if (!channel.Pending && channel.Lease == null)
                    return channel;

            if (effects.Count >= Mathf.Max(1, sfxCapacity)) return null;

            var created = new Channel();
            effects.Add(created);
            return created;
        }

        private AudioSource CreateSource(bool shouldLoop)
        {
            if (sourceHost == null)
            {
                sourceHost = new GameObject("Audio Sources");
                sourceHost.transform.SetParent(transform, false);
            }

            var source = sourceHost.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = shouldLoop;
            source.spatialBlend = 0f;
            return source;
        }

        private void Update()
        {
            if (AudioListener.pause) return;

            foreach (var effect in effects)
                if (effect.Lease != null && (effect.Source == null || !effect.Source.isPlaying))
                    effect.Clear();
        }

        private void StopEffects()
        {
            StopLoop();
            foreach (var effect in effects)
                effect.Stop();
        }

        private void OnDisable()
        {
            StopMusic();
            StopEffects();
        }

        private void OnDestroy() => Shutdown();

        private static float NormalizeVolume(float value) => float.IsNaN(value) ? 1f : Mathf.Clamp01(value);

        private sealed class Channel
        {
            public AudioSource Source;
            public AssetLease<AudioClip> Lease;
            public int Request;
            public bool Pending;

            public void Stop()
            {
                Request++;
                Pending = false;
                Clear();
            }

            public void Clear()
            {
                if (Source != null)
                {
                    Source.Stop();
                    Source.clip = null;
                }

                Lease?.Dispose();
                Lease = null;
            }

            public void SetVolume(float value)
            {
                if (Source != null) Source.volume = value;
            }
        }
    }
}
