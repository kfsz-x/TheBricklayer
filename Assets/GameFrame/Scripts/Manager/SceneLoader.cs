using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityCore
{
    public sealed class SceneLoader
    {
        public bool IsLoading { get; private set; }

        public async Task LoadAsync(string scenePath, Action<float> reportProgress = null)
        {
            if (IsLoading) throw new InvalidOperationException("A scene transition is already running.");
            if (string.IsNullOrEmpty(scenePath) || SceneUtility.GetBuildIndexByScenePath(scenePath) < 0)
                throw new ArgumentException("The scene must be enabled in Build Settings.", nameof(scenePath));

            IsLoading = true;
            try
            {
                Report(0f);
                var operation = SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Single);
                if (reportProgress != null)
                {
                    operation.allowSceneActivation = false;
                    try
                    {
                        while (operation.progress < .9f)
                        {
                            Report(operation.progress);
                            await Task.Yield();
                        }
                        // A frame at 100% lets the outgoing progress view render before it unloads.
                        Report(1f);
                        await Task.Yield();
                    }
                    finally { operation.allowSceneActivation = true; }
                }
                while (!operation.isDone)
                {
                    await Task.Yield();
                }
            }
            finally
            {
                IsLoading = false;
            }

            void Report(float progress)
            {
                try { reportProgress?.Invoke(progress); }
                catch (Exception exception) { Debug.LogException(exception); }
            }
        }
    }
}
