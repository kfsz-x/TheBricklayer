using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnityCore.Samples
{
    public sealed class MainPanel : MonoBehaviour
    {
        public const string ScenePath = "Assets/GameFrame/Game.unity";
        public const string CubeKey = "sample/cube";
        public const string ClickKey = "sample/click";
        public const string MusicKey = "sample/music";

        [Header("Game UI")]
        [SerializeField] private GameObject gameMenu;
        [SerializeField] private GameObject gameplayDemo;
        [SerializeField] private Text menuStatus;
        [SerializeField] private Text demoStatus;
        [SerializeField] private PanelView settings;
        [SerializeField] private Text musicToggleText;
        [SerializeField] private Text sfxToggleText;
        [SerializeField] private Text musicVolumeText;
        [SerializeField] private Text sfxVolumeText;

        [Header("Project actions")]
        [Tooltip("Enabled gameplay scene path in Build Settings. Leave empty to open the template demo.")]
        [SerializeField] private string gameplayScene;
        [SerializeField] private UnityEvent addDesktop = new();
        [SerializeField] private UnityEvent entryReward = new();
        [SerializeField] private UnityEvent share = new();
        [SerializeField] private UnityEvent subscribe = new();

        private CoreRoot core;
        private PrefabPool pool;
        private AssetLease<GameObject> cubeAsset;
        private GameObject cube;
        private int previousCube;
        private bool loadingCube;
        private void Start()
        {
            core = CoreRoot.GetOrCreate();
            pool = new PrefabPool(transform);
        }

        private void OnDestroy()
        {
            pool?.Dispose();
            cubeAsset?.Dispose();
        }

        public void BeginGame() => Run(async () =>
        {
            if (!string.IsNullOrWhiteSpace(gameplayScene))
            {
                if (core.Scenes.IsLoading) return;
                menuStatus.text = "正在进入游戏";
                await core.Scenes.LoadAsync(gameplayScene);
                return;
            }
            gameMenu.SetActive(false);
            gameplayDemo.SetActive(true);
        });

        public void ReturnToMenu()
        {
            if (cube != null)
            {
                pool.Return(cube);
                cube = null;
            }
            gameplayDemo.SetActive(false);
            gameMenu.SetActive(true);
            menuStatus.text = "";
        }

        public void RequestAddDesktop() => Request("添加桌面", addDesktop);
        public void RequestEntryReward() => Request("入口有奖", entryReward);
        public void RequestShare() => Request("分享", share);
        public void RequestSubscribe() => Request("订阅", subscribe);

        private void Request(string caption, UnityEvent action)
        {
            menuStatus.text = action.GetPersistentEventCount() == 0 ? $"{caption}功能待配置" : "";
            action.Invoke();
        }

        public void OpenSettings()
        {
            RefreshSettings();
            settings.Show();
        }

        public void ToggleMusic()
        {
            core.Audio.MusicEnabled = !core.Audio.MusicEnabled;
            RefreshSettings();
        }

        public void ToggleEffects()
        {
            core.Audio.SfxEnabled = !core.Audio.SfxEnabled;
            RefreshSettings();
        }

        public void StepMusicVolume()
        {
            core.Audio.MusicVolume = NextVolume(core.Audio.MusicVolume);
            RefreshSettings();
        }

        public void StepEffectsVolume()
        {
            core.Audio.SfxVolume = NextVolume(core.Audio.SfxVolume);
            RefreshSettings();
        }

        private static float NextVolume(float value) => value >= 1f ? 0f : Mathf.Min(1f, value + .25f);

        private void RefreshSettings()
        {
            musicToggleText.text = core.Audio.MusicEnabled ? "音乐：开启" : "音乐：关闭";
            sfxToggleText.text = core.Audio.SfxEnabled ? "音效：开启" : "音效：关闭";
            musicVolumeText.text = $"音乐音量：{Mathf.RoundToInt(core.Audio.MusicVolume * 100f)}%";
            sfxVolumeText.text = $"音效音量：{Mathf.RoundToInt(core.Audio.SfxVolume * 100f)}%";
        }

        public void PlaySampleEffect() => Run(() => core.Audio.PlaySfxAsync(ClickKey));
        public void PlaySampleMusic() => Run(() => core.Audio.PlayMusicAsync(MusicKey));
        public void StopSampleMusic() => core.Audio.StopMusic();
        public void ToggleSampleCube() => Run(ToggleCube);

        private async Task ToggleCube()
        {
            if (loadingCube) return;
            if (cube != null)
            {
                pool.Return(cube);
                cube = null;
                demoStatus.text = "已归还方块";
                return;
            }

            loadingCube = true;
            try
            {
                if (cubeAsset == null)
                {
                    var loaded = await core.Resources.AcquireAsync<GameObject>(CubeKey);
                    if (this == null)
                    {
                        loaded.Dispose();
                        return;
                    }
                    cubeAsset = loaded;
                }
                if (!gameplayDemo.activeSelf) return;

                cube = pool.Rent(cubeAsset.Asset);
                cube.transform.SetPositionAndRotation(new Vector3(2.1f, 0f, 0f), Quaternion.Euler(18f, 30f, 0f));
                var id = cube.GetInstanceID();
                demoStatus.text = id == previousCube ? "已复用方块" : "已加载方块";
                previousCube = id;
            }
            finally { loadingCube = false; }
        }

        private async void Run(Func<Task> action)
        {
            try { await action(); }
            catch (Exception exception)
            {
                if (this != null)
                {
                    var message = gameplayDemo.activeSelf ? demoStatus : menuStatus;
                    message.text = "操作失败，请检查配置";
                }
                Debug.LogException(exception);
            }
        }
    }
}
