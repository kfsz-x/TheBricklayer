using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEditor.Build;

namespace QGMiniGameCore
{
    public class QGGameBuild : IUnityCompatible
    {

        private static QGGameBuild instance;
        public static QGGameBuild Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new QGGameBuild();
                }
                return instance;
            }
        }
        public bool SetPlayer(bool useWebgl2)
        {
            PlayerSettings.runInBackground = false;
            PlayerSettings.allowUnsafeCode = true;
            PlayerSettings.WebGL.threadsSupport = false;
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.WebGL, false);
            GraphicsDeviceType[] targets = { useWebgl2 ? GraphicsDeviceType.OpenGLES3 : GraphicsDeviceType.OpenGLES2 };
            PlayerSettings.SetGraphicsAPIs(BuildTarget.WebGL, targets);
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
            PlayerSettings.WebGL.template = "APPLICATION:Minimal";
            //影响部分游戏设置，需要去除
            // EditorSettings.spritePackerMode = SpritePackerMode.AlwaysOnAtlas;
            PlayerSettings.WebGL.linkerTarget = WebGLLinkerTarget.Wasm;
            PlayerSettings.WebGL.dataCaching = false;
            return true;
        }

        public bool BuildWebGL(string srcPath, QGGameConfig qgConfig)
        {
            QGLog.Log("[BuildWebGL] Start: Please Waitting");

            PlayerSettings.WebGL.emscriptenArgs = string.Empty;
            if (QGCoreEnv.QG_UNITY_2021_2_OR_NEWER)
            {
                PlayerSettings.WebGL.emscriptenArgs += " -s EXPORTED_FUNCTIONS=_sbrk,_emscripten_stack_get_base,_emscripten_stack_get_end";
#if UNITY_2021_2_5 || UNITY_6000_0_OR_NEWER
                PlayerSettings.WebGL.emscriptenArgs += ",_main";
#endif
                PlayerSettings.WebGL.emscriptenArgs += " -s ERROR_ON_UNDEFINED_SYMBOLS=0";
            }

            if (qgConfig.projectConf.memorySize != 0)
            {
                if (qgConfig.projectConf.memorySize >= 1024)
                {
                    UnityEngine.Debug.LogErrorFormat($"UnityHeap必须小于1024。");
                }
                else if (qgConfig.projectConf.memorySize >= 500)
                {
                    UnityEngine.Debug.LogWarningFormat($"UnityHeap大于500M时，32位Android较大概率启动失败，中轻度游戏建议小于该值。");
                }
                PlayerSettings.WebGL.emscriptenArgs += $" -s TOTAL_MEMORY={qgConfig.projectConf.memorySize}MB";
            }

            string original_EXPORTED_RUNTIME_METHODS = "\"ccall\",\"cwrap\",\"stackTrace\",\"addRunDependency\",\"removeRunDependency\",\"FS_createPath\",\"FS_createDataFile\",\"stackTrace\",\"writeStackCookie\",\"checkStackCookie\"";
            // 添加额外的EXPORTED_RUNTIME_METHODS
            string additional_EXPORTED_RUNTIME_METHODS = ",\"lengthBytesUTF8\",\"stringToUTF8\"";

            PlayerSettings.WebGL.emscriptenArgs += " -s EXPORTED_RUNTIME_METHODS='[" + original_EXPORTED_RUNTIME_METHODS + additional_EXPORTED_RUNTIME_METHODS + "]'";

            if (qgConfig.projectConf.profilingFuncs)
            {
                PlayerSettings.WebGL.emscriptenArgs += " --profiling-funcs ";
            }

#if UNITY_2021_2_OR_NEWER
#if UNITY_2022_1_OR_NEWER
                // 默认更改为OptimizeSize，减少代码包体积
            PlayerSettings.SetIl2CppCodeGeneration(NamedBuildTarget.WebGL, qgConfig.projectConf.il2CppOptimizeSize ? Il2CppCodeGeneration.OptimizeSize : Il2CppCodeGeneration.OptimizeSpeed);
#else
            EditorUserBuildSettings.il2CppCodeGeneration = qgConfig.projectConf.il2CppOptimizeSize ? Il2CppCodeGeneration.OptimizeSize : Il2CppCodeGeneration.OptimizeSpeed;
#endif
#endif
            UnityEngine.Debug.Log("[Builder] Starting to build WebGL project ... ");
            UnityEngine.Debug.Log("PlayerSettings.WebGL.emscriptenArgs : " + PlayerSettings.WebGL.emscriptenArgs);

            // PlayerSettings.WebGL.memorySize = memorySize;
            BuildOptions option = BuildOptions.None;

            if (qgConfig.projectConf.scriptOnly)
            {
                option |= BuildOptions.BuildScriptsOnly;
            }
#if UNITY_2021_2_OR_NEWER
            if (qgConfig.projectConf.cleanBuild)
            {
                option |= BuildOptions.CleanBuildCache;
            }
#endif
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.WebGL)
            {
                UnityEngine.Debug.LogFormat("[Builder] Current target is: {0}, switching to: {1}", EditorUserBuildSettings.activeBuildTarget, BuildTarget.WebGL);
                if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL))
                {
                    UnityEngine.Debug.LogFormat("[Builder] Switching to {0}/{1} failed!", BuildTargetGroup.WebGL, BuildTarget.WebGL);
                    return false;
                }
            }

#if UNITY_2021_2_OR_NEWER
            PlayerSettings.WebGL.debugSymbolMode = WebGLDebugSymbolMode.External;
#else
            PlayerSettings.WebGL.debugSymbols = true;
#endif

            var result = BuildPipeline.BuildPlayer(QGGameTools.GetScenePaths(), srcPath, BuildTarget.WebGL, option);
            if (result.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                UnityEngine.Debug.LogFormat("[Builder] BuildPlayer failed. emscriptenArgs:{0}", PlayerSettings.WebGL.emscriptenArgs);
            }

            QGLog.Log("[BuildWebGL] Done: " + srcPath);
            return true;
        }

        void IUnityCompatible.OnZipFile(string zipOutPath, Dictionary<string, string> fileMap)
        {
            using (FileStream fs = new FileStream(zipOutPath, FileMode.Create))
            using (ZipArchive archive = new ZipArchive(fs, ZipArchiveMode.Create))
            {
                foreach (var pair in fileMap)
                {
                    string sourceFilePath = pair.Key;
                    string entryName = pair.Value;

                    if (!File.Exists(sourceFilePath))
                        throw new FileNotFoundException($"源文件不存在: {sourceFilePath}");

                    ZipArchiveEntry entry = archive.CreateEntry(entryName);
                    using (FileStream fileStream = File.OpenRead(sourceFilePath))
                    using (Stream entryStream = entry.Open())
                    {
                        fileStream.CopyTo(entryStream);
                    }
                }
            }
        }

        public void DoBuild()
        {
            QGEditorWindowNew.OnInitEnv();
            QGWindowHepler.Instance.SetUnityCompatible(Instance);
            QGWindowHepler.Instance.SetIsBuildRpk(true);
            QGWindowHepler.Instance.OnConfigSetting();
            QGWindowHepler.Instance.DoBuildCMD();
        }
    }
}
