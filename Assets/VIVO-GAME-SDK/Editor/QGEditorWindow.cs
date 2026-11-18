using UnityEditor;
using UnityEngine;

namespace QGMiniGameCore
{
    public class QGEditorWindowNew : EditorWindow
    {
        QGWindowHepler qGWindowHepler;

        [MenuItem("VIVO小游戏 / 转换小游戏", false, 1)]
        public static void OpenWindow()
        {
            QGLog.Log("OpenWindow");
            var win = GetWindow(typeof(QGEditorWindowNew), false, "vivo小游戏转换工具面板");
            win.minSize = new Vector2(350, 400);
            win.Show();
            QGGameTools.CheckUpdate();
        }

        private void OnEnable()
        {
            OnInitEnv();
            EditorUtility.ClearProgressBar();
            qGWindowHepler = new QGWindowHepler(this);
            qGWindowHepler.SetUnityCompatible(QGGameBuild.Instance);
            qGWindowHepler.SetIsBuildRpk(true);
            QGLog.Log("OnEnable");
            qGWindowHepler.OnConfigSetting();
            if (!QGCoreEnv.QG_UNITY_2019_1_OR_NEWER)
            {
                QGLog.LogError("目前仅支持 Unity2019以上的版本！");
            }
        }

        // 打包构建可编辑面板UI
        private void OnGUI()
        {
            qGWindowHepler.OnGUISetting();
        }

        private void OnDisable()
        {
            QGWindowHepler.Instance.OnConfigUpdate();
        }

        public static void OnInitEnv()
        {
#if UNITY_2019
            QGCoreEnv.QG_UNITY_2019 = true;
#endif
#if UNITY_2019_1_OR_NEWER
            QGCoreEnv.QG_UNITY_2019_1_OR_NEWER = true;
#endif
#if UNITY_2020_1_OR_NEWER
            QGCoreEnv.QG_UNITY_2020_1_OR_NEWER = true;
#endif
#if UNITY_2020_3_OR_NEWER
            QGCoreEnv.QG_UNITY_2020_3_OR_NEWER = true;
#endif
#if UNITY_2021_1_OR_NEWER
            QGCoreEnv.QG_UNITY_2021_1_OR_NEWER = true;
#endif
#if UNITY_2021_2_OR_NEWER
            QGCoreEnv.QG_UNITY_2021_2_OR_NEWER = true;
#endif
#if UNITY_2021_3_OR_NEWER
            QGCoreEnv.QG_UNITY_2021_3_OR_NEWER = true;
#endif
#if TUANJIE_2022_3_OR_NEWER
            QGCoreEnv.QG_TUANJIE_2022_3_OR_NEWER = true;
#endif
#if UNITY_2021
            QGCoreEnv.QG_UNITY_2021 = true;
#endif
#if UNITY_2022
            QGCoreEnv.QG_UNITY_2022 = true;
#endif
        }

    }
}
