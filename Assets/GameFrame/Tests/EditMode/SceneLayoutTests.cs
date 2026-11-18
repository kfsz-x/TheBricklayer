using System;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UnityCore.Tests
{
    public class SceneLayoutTests
    {
        [Test]
        public void BuiltInScenesContainEditableUiBeforePlay()
        {
            for (var i = 0; i < SceneManager.sceneCount; i++)
                if (SceneManager.GetSceneAt(i).isDirty)
                    Assert.Ignore("Save open scenes before running this scene layout test.");

            var setup = EditorSceneManager.GetSceneManagerSetup();
            try
            {
                var scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).ToArray();
                Assert.That(scenes.Length, Is.GreaterThanOrEqualTo(2));
                VerifyScene(scenes[0].path, (init, roots) =>
                {
                    Assert.That(roots.Any(root => root.GetComponent<Canvas>() != null), Is.True, "Init Canvas");
                    Assert.That(roots.SelectMany(root => root.GetComponentsInChildren<Slider>(true))
                        .Any(slider => slider.name == "Progress" && slider.fillRect != null), Is.True);
                    Assert.That(roots.SelectMany(root => root.GetComponentsInChildren<Text>(true))
                        .Any(label => label.name == "Version" && label.font != null), Is.True);
                    Assert.That(init.name, Is.EqualTo("Init"));
                    var canvas = roots.Single(root => root.name == "MainCanvas").transform;
                    Assert.That(canvas.Find("Bg").GetComponent<Image>().sprite.name, Is.EqualTo("bg"));
                    var loading = canvas.Find("Loading");
                    Assert.That(loading.Find("Title"), Is.Null);
                    Assert.That(loading.GetComponent<Image>(), Is.Null);
                    var progress = loading.Find("Progress");
                    Assert.That(progress.GetComponent<Image>().sprite.name, Is.EqualTo("jdt2"));
                    Assert.That(progress.GetComponent<Slider>().fillRect.GetComponent<Image>().sprite.name,
                        Is.EqualTo("jdt1"));
                });

                VerifyScene(scenes[1].path, (game, roots) =>
                {
                    Assert.That(game.name, Is.EqualTo("Game"));
                    Assert.That(roots.Any(root => root.GetComponent<EventSystem>() != null), Is.True);
                    var menu = roots.SelectMany(root => root.GetComponentsInChildren<Transform>(true))
                        .Single(node => node.name == "MainPanel");
                    var buttons = menu.GetComponentsInChildren<Button>(true);
                    CollectionAssert.AreEquivalent(new[] { "设置", "开始游戏", "添加桌面", "入口有奖", "分享", "订阅" },
                        buttons.Select(button => button.GetComponentInChildren<Text>(true).text));
                    Assert.That(buttons.All(button => button.onClick.GetPersistentEventCount() > 0 &&
                        button.onClick.GetPersistentTarget(0) != null), Is.True);
                    Assert.That(menu.Find("Title"), Is.Null);
                    Assert.That(menu.GetComponent<Image>(), Is.Null);
                    Assert.That(menu.parent.Find("Bg").GetComponent<Image>().sprite.name, Is.EqualTo("bjt"));
                    var setting = (RectTransform)menu.Find("SettingBtn");
                    Assert.That(setting.anchorMin, Is.EqualTo(new Vector2(0f, 1f)));
                    var channel = (RectTransform)menu.Find("ChannelPanel");
                    Assert.That(channel.anchorMin, Is.EqualTo(new Vector2(0f, 1f)));
                    var ordered = new[] { "AddDeskBtn", "RewardBtn", "ShareBtn", "SubscribeBtn" }
                        .Select(name => (RectTransform)channel.Find(name)).ToArray();
                    for (var i = 1; i < ordered.Length; i++)
                    {
                        Assert.That(ordered[i].anchoredPosition.x, Is.EqualTo(ordered[0].anchoredPosition.x));
                        Assert.That(ordered[i].anchoredPosition.y, Is.LessThan(ordered[i - 1].anchoredPosition.y));
                    }
                    var start = (RectTransform)menu.Find("StartBtn");
                    Assert.That(start.anchorMin, Is.EqualTo(new Vector2(.5f, 0f)));
                    Assert.That(start.anchoredPosition.x, Is.Zero);
                    Assert.That(start.GetComponent<Image>().sprite.name, Is.EqualTo("anniu"));
                });
            }
            finally
            {
                if (setup.Any(scene => !string.IsNullOrEmpty(scene.path)))
                    EditorSceneManager.RestoreSceneManagerSetup(setup);
                else
                    EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            }
        }

        private static void VerifyScene(string path, Action<Scene, GameObject[]> verify)
        {
            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            verify(scene, scene.GetRootGameObjects());
        }
    }
}
