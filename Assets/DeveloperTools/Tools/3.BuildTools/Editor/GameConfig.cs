using System;
using UnityEditor;
using UnityEngine;

public enum PlatformType
{
    Android,
    IOS,
    Windows,
    Dll,
    UnityPackage
}

// Build and import preferences for DeveloperTools. The asset lives in an Editor folder.
public sealed class GameConfig : ScriptableObject
{
    private const string AssetPath = "Assets/DeveloperTools/Tools/3.BuildTools/Editor/GameConfig.asset";
    private static GameConfig current;

    public static GameConfig Instance
    {
        get
        {
            if (current != null) return current;
            current = AssetDatabase.LoadAssetAtPath<GameConfig>(AssetPath);
            if (current != null) return current;

            current = CreateInstance<GameConfig>();
            current.CompanyName = PlayerSettings.companyName;
            current.AppName = PlayerSettings.productName;
            current.VersionNum = PlayerSettings.bundleVersion;
            current.BundleVersionNum = PlayerSettings.Android.bundleVersionCode;
            current.PackageName = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Standalone);
            return current;
        }
    }

    public string CompanyName = "";
    public string AppName = "";
    public string ChannelName = "";
    public string VersionNum = "1.0.0";
    public int BundleVersionNum = 1;
    public string PackageName = "";
    public bool isSetImg;
    public bool isSetScreenLogo;
    public bool islockFPS;
    public bool isRelease;
    public bool is32;
    public bool isBuildAab;
    public bool isOnlyBuild = true;
    public PlatformType PT = PlatformType.Windows;
    public SDKPath[] SDKPath = new SDKPath[3];
    public Texture[] TexAsset = new Texture[4];
    public string KeyName = "Key.jks";
    [HideInInspector] public string KeyPassWord = "";

    public Texture GetTexture(string name)
    {
        int index;
        switch (name)
        {
            case "Icon": index = 0; break;
            case "Splash": index = 1; break;
            case "WX": index = 2; break;
            case "Lion": index = 3; break;
            default: return null;
        }
        return TexAsset != null && index < TexAsset.Length ? TexAsset[index] : null;
    }

    [MenuItem("Tools/DeveloperTools/Build Settings")]
    public static void SelectSettings()
    {
        var config = Instance;
        if (!EditorUtility.IsPersistent(config))
        {
            AssetDatabase.CreateAsset(config, AssetPath);
            AssetDatabase.SaveAssets();
        }
        Selection.activeObject = config;
    }
}

[Serializable]
public struct SDKPath
{
    public string SDKName;
    public string SdkPath;
}
