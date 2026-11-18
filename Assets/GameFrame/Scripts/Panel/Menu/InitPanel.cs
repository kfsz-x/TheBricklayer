using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UnityCore.Samples
{
    public sealed class InitPanel : MonoBehaviour
    {
        public const string ScenePath = "Assets/GameFrame/Init.unity";

        [SerializeField] private Slider progress;
        [SerializeField] private Text version;
        [SerializeField] private Text loadingStatus;
        [SerializeField] private Button retry;

        private CoreRoot core;
        private bool loadingInit;

        private void Start()
        {
            core = CoreRoot.GetOrCreate();
            version.text = $"版本号 {Application.version}";
            RetryLoading();
        }

        public async void RetryLoading()
        {
            if (loadingInit) return;
            loadingInit = true;
            retry.gameObject.SetActive(false);
            SetProgress(0f);
            try
            {
                do { await Task.Yield(); }
                while (this != null && core.Scenes.IsLoading);
                if (this != null) await core.Scenes.LoadAsync(MainPanel.ScenePath, SetProgress);
            }
            catch (Exception exception)
            {
                if (this == null) return;
                loadingStatus.text = "加载失败，请重试";
                retry.gameObject.SetActive(true);
                Debug.LogException(exception);
            }
            finally { loadingInit = false; }
        }

        private void SetProgress(float value)
        {
            if (progress == null) return;
            progress.SetValueWithoutNotify(value);
            loadingStatus.text = $"正在加载 {Mathf.RoundToInt(value * 100f)}%";
        }
    }
}
