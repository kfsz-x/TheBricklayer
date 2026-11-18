using UnityEditor;
public static class SceneList
{
    [MenuItem("场景列表/Init", priority = 10)]
    public static void Init0() { ScenesMenu.OpenScene("Assets/GameFrame/Init.unity"); }
    [MenuItem("场景列表/Game", priority = 10)]
    public static void Game1() { ScenesMenu.OpenScene("Assets/GameFrame/Game.unity"); }
}

