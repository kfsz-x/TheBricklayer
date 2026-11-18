using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace UnityCore.Tests
{
    public class SceneTests
    {
        private const string Init = "Assets/GameFrame/Init.unity";
        private const string Game = "Assets/GameFrame/Game.unity";

        [UnityTest]
        public IEnumerator StartupShowsProgressAndVersionThenOpensGameMenu()
        {
            yield return SceneManager.LoadSceneAsync(Init);
            var startupDeadline = Time.realtimeSinceStartup + 1f;
            yield return new WaitUntil(() => GameObject.Find("Progress") != null ||
                Time.realtimeSinceStartup >= startupDeadline);
            var progress = GameObject.Find("Progress");
            Assert.That(progress, Is.Not.Null);
            Assert.That(progress.GetComponent<Slider>().normalizedValue, Is.InRange(0f, 1f));
            Assert.That(GameObject.Find("Version").GetComponent<Text>().text, Does.Contain(Application.version));

            var core = CoreRoot.Current;
            var deadline = Time.realtimeSinceStartup + 10f;
            yield return new WaitUntil(() => GameObject.Find("StartBtn") != null ||
                Time.realtimeSinceStartup >= deadline);
            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo("Game"));
            Assert.That(CoreRoot.Current, Is.SameAs(core));

            var actions = new[] { "SettingBtn", "StartBtn", "AddDeskBtn", "RewardBtn", "ShareBtn", "SubscribeBtn" };
            var menu = GameObject.Find("MainPanel");
            Assert.That(menu, Is.Not.Null);
            CollectionAssert.AreEquivalent(actions, menu.GetComponentsInChildren<Button>()
                .Select(button => button.gameObject.name));
            foreach (var button in menu.GetComponentsInChildren<Button>())
            {
                var label = button.GetComponentInChildren<Text>();
                Assert.That(label.text.All(label.font.HasCharacter), Is.True, label.text);
            }
            Click("SettingBtn");
            Assert.That(GameObject.Find("CloseBtn"), Is.Not.Null);
            Click("CloseBtn");
            Click("ShareBtn");
            Assert.That(GameObject.Find("Tips").GetComponent<Text>().text, Does.Contain("分享"));
            Click("StartBtn");
            Assert.That(GameObject.Find("BackBtn"), Is.Not.Null);
            Click("BackBtn");
            Assert.That(GameObject.Find("StartBtn"), Is.Not.Null);
        }

        [UnityTest]
        public IEnumerator SceneLoaderRejectsOverlapAndReportsReady()
        {
            var core = CoreRoot.GetOrCreate();
            var progress = 0f;
            var load = core.Scenes.LoadAsync(Game, value => progress = value);
            var overlap = core.Scenes.LoadAsync(Init);
            Assert.Throws<InvalidOperationException>(() => overlap.GetAwaiter().GetResult());
            yield return new WaitUntil(() => load.IsCompleted);
            load.GetAwaiter().GetResult();
            Assert.That(progress, Is.EqualTo(1f));
            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo("Game"));
            Assert.That(CoreRoot.Current, Is.SameAs(core));
        }

        private static void Click(string name) =>
            GameObject.Find(name).GetComponent<Button>().onClick.Invoke();

        [UnityTearDown]
        public IEnumerator DestroyCore()
        {
            if (CoreRoot.Current != null)
                UnityEngine.Object.Destroy(CoreRoot.Current.gameObject);
            yield return null;
        }
    }
}


