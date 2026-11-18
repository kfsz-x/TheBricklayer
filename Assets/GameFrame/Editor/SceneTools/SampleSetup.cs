using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

namespace UnityCore.Samples.Editor
{
    public static class SampleSetup
    {
        private const string DemoPrefabs = "Assets/GameFrame/Prefab/Demo";
        private const string CommonAudios = "Assets/GameFrame/Audios/com";

        [MenuItem("Tools/Unity Core/Samples/Ensure Sample Assets")]
        public static void Create()
        {
            if ((!File.Exists(InitPanel.ScenePath) || !File.Exists(MainPanel.ScenePath)) &&
                !Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            Directory.CreateDirectory(DemoPrefabs);
            Directory.CreateDirectory(CommonAudios);
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            var prefab = DemoPrefabs + "/Cube.prefab";
            if (!File.Exists(prefab))
            {
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.name = "Sample Cube";
                PrefabUtility.SaveAsPrefabAsset(cube, prefab);
                UnityEngine.Object.DestroyImmediate(cube);
            }

            var click = CommonAudios + "/btn.wav";
            var music = CommonAudios + "/bgmmain.wav";
            if (!File.Exists(click)) WriteTone(click, .12, 660, false);
            if (!File.Exists(music)) WriteTone(music, 2, 220, true);
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                settings = AddressableAssetSettings.Create("Assets/AddressableAssetsData",
                    "AddressableAssetSettings", true, true);
                AddressableAssetSettingsDefaultObject.Settings = settings;
            }

            settings.BuildAddressablesWithPlayerBuild = AddressableAssetSettings.PlayerBuildOption.BuildWithPlayer;
            var group = settings.FindGroup("Template Samples") ?? settings.CreateGroup("Template Samples",
                false, false, false, null, typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema));
            settings.AddLabel("sample-audio");
            Register(prefab, MainPanel.CubeKey);
            Register(click, MainPanel.ClickKey, "sample-audio");
            Register(music, MainPanel.MusicKey, "sample-audio");
            EditorUtility.SetDirty(settings);

            CreateScene(MainPanel.ScenePath, true);
            CreateScene(InitPanel.ScenePath, false);
            var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            var homeIndex = scenes.FindIndex(scene => scene.path == InitPanel.ScenePath);
            var nextIndex = scenes.FindIndex(scene => scene.path == MainPanel.ScenePath);
            if (homeIndex < 0)
            {
                homeIndex = nextIndex >= 0 ? nextIndex : scenes.Count;
                scenes.Insert(homeIndex, new EditorBuildSettingsScene(InitPanel.ScenePath, true));
                if (nextIndex >= 0) nextIndex++;
            }
            else scenes[homeIndex].enabled = true;

            if (nextIndex < 0) scenes.Add(new EditorBuildSettingsScene(MainPanel.ScenePath, true));
            else
            {
                scenes[nextIndex].enabled = true;
                if (homeIndex > nextIndex)
                {
                    var start = scenes[homeIndex];
                    scenes.RemoveAt(homeIndex);
                    scenes.Insert(nextIndex, start);
                }
            }
            EditorBuildSettings.scenes = scenes.ToArray();
            AssetDatabase.SaveAssets();
            if (Application.isBatchMode) EditorSceneManager.OpenScene(InitPanel.ScenePath);
            Debug.Log("UnityCore sample assets are ready.");

            void Register(string path, string key, string label = null)
            {
                var entry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), group);
                entry.SetAddress(key);
                if (label != null) entry.SetLabel(label, true);
                EditorUtility.SetDirty(group);
            }
        }

        private static void CreateScene(string path, bool second)
        {
            if (File.Exists(path)) return;
            var old = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            var additive = old.IsValid() && !string.IsNullOrEmpty(old.path);
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,
                additive ? NewSceneMode.Additive : NewSceneMode.Single);
            UnityEngine.SceneManagement.SceneManager.SetActiveScene(scene);
            try
            {
                if (!second)
                    new GameObject("Core", typeof(CoreRoot));
                if (second)
                    SampleSceneUi.Build(null, new GameObject("GameUI").AddComponent<MainPanel>());
                else
                    SampleSceneUi.Build(new GameObject("InitUI").AddComponent<InitPanel>(), null);

                var camera = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
                camera.tag = "MainCamera";
                camera.transform.position = new Vector3(0f, 0f, -8f);
                var view = camera.GetComponent<Camera>();
                view.clearFlags = CameraClearFlags.SolidColor;
                view.backgroundColor = new Color(.14f, .22f, .31f);
                view.orthographic = true;
                view.orthographicSize = 3f;

                var light = new GameObject("Directional Light", typeof(Light));
                light.transform.rotation = Quaternion.Euler(45, -35, 0);
                light.GetComponent<Light>().type = LightType.Directional;
                if (!EditorSceneManager.SaveScene(scene, path))
                    throw new InvalidOperationException($"Could not save {path}.");
            }
            finally
            {
                if (additive && old.IsValid())
                {
                    UnityEngine.SceneManagement.SceneManager.SetActiveScene(old);
                    EditorSceneManager.CloseScene(scene, true);
                }
            }
        }

        private static void WriteTone(string path, double seconds, double frequency, bool music)
        {
            const int rate = 22050;
            var frames = (int)(rate * seconds);
            using var writer = new BinaryWriter(File.Create(path));
            writer.Write(new[] { 'R', 'I', 'F', 'F' });
            writer.Write(36 + frames * 2);
            writer.Write(new[] { 'W', 'A', 'V', 'E', 'f', 'm', 't', ' ' });
            writer.Write(16);
            writer.Write((short)1);
            writer.Write((short)1);
            writer.Write(rate);
            writer.Write(rate * 2);
            writer.Write((short)2);
            writer.Write((short)16);
            writer.Write(new[] { 'd', 'a', 't', 'a' });
            writer.Write(frames * 2);
            for (int i = 0; i < frames; i++)
            {
                var t = (double)i / rate;
                var envelope = music ? Math.Pow(Math.Sin(Math.PI * t / seconds), 2) : 1 - t / seconds;
                var phase = 2 * Math.PI * frequency * t;
                var sample = Math.Sin(phase);
                if (music) sample = (sample + Math.Sin(phase * 1.5) + Math.Sin(phase * 2)) / 3;
                writer.Write((short)(sample * envelope * 6500));
            }
        }
    }
}


