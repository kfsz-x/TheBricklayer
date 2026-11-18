using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace UnityCore.Editor
{
    public static class ProjectBuild
    {
        [MenuItem("Tools/Unity Core/Build Windows x64")]
        public static void BuildWindows()
        {
            var scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();
            if (scenes.Length == 0)
                throw new InvalidOperationException("Enable a startup scene in Build Settings.");
            foreach (var scene in scenes)
                if (AssetDatabase.LoadAssetAtPath<SceneAsset>(scene) == null)
                    throw new InvalidOperationException($"Build Settings contains a missing scene: {scene}.");

            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows64)
                throw new InvalidOperationException("Switch to Windows x86_64 in Build Settings before building.");

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings != null)
            {
                AddressableAssetSettings.BuildPlayerContent(out var result);
                if (!string.IsNullOrEmpty(result.Error))
                    throw new InvalidOperationException(result.Error);
            }

            Directory.CreateDirectory("Builds/Windows");
            var previous = settings != null ? settings.BuildAddressablesWithPlayerBuild : default;
            try
            {
                if (settings != null)
                    settings.BuildAddressablesWithPlayerBuild = AddressableAssetSettings.PlayerBuildOption.DoNotBuildWithPlayer;

                var report = BuildPipeline.BuildPlayer(scenes, "Builds/Windows/UnityCoreTemplate.exe",
                    BuildTarget.StandaloneWindows64, BuildOptions.None);
                if (report.summary.result != BuildResult.Succeeded || report.summary.totalErrors != 0)
                    throw new InvalidOperationException($"Player build failed: {report.summary.result}.");
                Debug.Log($"UnityCore Windows build succeeded ({report.summary.totalSize:N0} bytes).");
            }
            finally
            {
                if (settings != null) settings.BuildAddressablesWithPlayerBuild = previous;
            }
        }
    }
}
