using System;
using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace UnityCore.Tests
{
    public sealed class AudioUiTests
    {
        private GameObject host;
        private float originalTimeScale;

        [SetUp]
        public void SetUp()
        {
            host = new GameObject(nameof(AudioUiTests));
            originalTimeScale = Time.timeScale;
        }

        [TearDown]
        public void TearDown()
        {
            Time.timeScale = originalTimeScale;
            Object.DestroyImmediate(host);
        }

        [UnityTest]
        public IEnumerator StopMusicInvalidatesAPendingPlayback()
        {
            using var resources = new ResourceStore();
            var audio = AddAudio(resources);
            var pending = audio.PlayMusicAsync("sample/music");

            audio.StopMusic();

            yield return new WaitUntil(() => pending.IsCompleted);
            pending.GetAwaiter().GetResult();
            AssertSourcesHaveNoClips();
        }

        [UnityTest]
        public IEnumerator ShutdownClearsActiveMusicAndInvalidatesPendingLoop()
        {
            using var resources = new ResourceStore();
            var audio = AddAudio(resources);
            var music = audio.PlayMusicAsync("sample/music");
            yield return new WaitUntil(() => music.IsCompleted);
            music.GetAwaiter().GetResult();
            Assert.That(host.GetComponentInChildren<AudioSource>().clip, Is.Not.Null);

            var loop = audio.PlayLoopAsync("sample/click");
            audio.Shutdown();
            resources.Dispose();

            yield return new WaitUntil(() => loop.IsCompleted);
            loop.GetAwaiter().GetResult();
            AssertSourcesHaveNoClips();
        }

        [UnityTest]
        public IEnumerator BurstOfOneShotsUsesABoundedNumberOfSources()
        {
            using var resources = new ResourceStore();
            var audio = AddAudio(resources);
            var requests = new Task[24];
            for (var index = 0; index < requests.Length; index++)
                requests[index] = audio.PlaySfxAsync("sample/click");

            var burst = Task.WhenAll(requests);
            yield return new WaitUntil(() => burst.IsCompleted);
            burst.GetAwaiter().GetResult();
            Assert.That(host.GetComponentsInChildren<AudioSource>().Length, Is.InRange(1, 8));
            audio.Shutdown();
        }

        [UnityTest]
        public IEnumerator ReopeningPanelCancelsItsPendingHide()
        {
            var panel = host.AddComponent<PanelView>();
            var group = host.GetComponent<CanvasGroup>();
            panel.Hide(0.05f);
            Assert.That(group.blocksRaycasts, Is.False);

            panel.Show(0f);
            yield return new WaitForSecondsRealtime(0.15f);

            Assert.That(host.activeSelf, Is.True);
            Assert.That(group.alpha, Is.EqualTo(1f));
            Assert.That(group.interactable, Is.True);
            Assert.That(group.blocksRaycasts, Is.True);
        }

        [UnityTest]
        public IEnumerator PanelHidesWhileGameplayTimeIsPaused()
        {
            var panel = host.AddComponent<PanelView>();
            Time.timeScale = 0f;

            panel.Hide(0.05f);
            Assert.That(host.activeSelf, Is.True);
            yield return new WaitForSecondsRealtime(0.15f);

            Assert.That(host.activeSelf, Is.False);
            Assert.That(host.GetComponent<CanvasGroup>().alpha, Is.Zero);
        }

        private AudioService AddAudio(ResourceStore resources)
        {
            var audio = host.AddComponent<AudioService>();
            audio.Initialize(resources, new LocalStore($"test.audio.{Guid.NewGuid():N}"));
            return audio;
        }

        private void AssertSourcesHaveNoClips()
        {
            foreach (var source in host.GetComponentsInChildren<AudioSource>(true))
                Assert.That(source.clip, Is.Null);
        }
    }
}
