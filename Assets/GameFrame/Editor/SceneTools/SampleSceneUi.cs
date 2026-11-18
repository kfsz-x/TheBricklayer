using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityCore.Samples.Editor
{
    internal static class SampleSceneUi
    {
        private static readonly Color CardColor = new(.055f, .095f, .15f, .97f);
        private static readonly Color ButtonColor = new(.15f, .27f, .40f);
        private static readonly Color Accent = new(.13f, .75f, .88f);
        private static readonly Vector2 Center = new(.5f, .5f);

        internal static void Build(InitPanel init, MainPanel main)
        {
            var serialized = new SerializedObject(init != null ? (Object)init : main);
            var font = AssetDatabase.LoadAssetAtPath<Font>("Assets/GameFrame/Fonts/MenuFont.otf");
            if (font == null) font = Resources.GetBuiltinResource<Font>("Arial.ttf");

            var canvas = Rect(null, "MainCanvas", Center, Vector2.zero, Vector2.zero);
            canvas.gameObject.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(960, 600);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            canvas.gameObject.AddComponent<GraphicRaycaster>();
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

            if (main != null) BuildGame();
            else BuildInit();
            ApplyTemplateArt(canvas, main != null);
            serialized.ApplyModifiedPropertiesWithoutUndo();

            void Bind(string property, Object target) =>
                serialized.FindProperty(property).objectReferenceValue = target;

            Text Label(Transform parent, string name, string value, int fontSize, Vector2 position, Vector2 size)
            {
                var node = Rect(parent, name, Center, position, size);
                var label = node.gameObject.AddComponent<Text>();
                label.font = font;
                label.text = value;
                label.fontSize = fontSize;
                label.alignment = TextAnchor.MiddleCenter;
                label.color = Color.white;
                label.raycastTarget = false;
                return label;
            }

            Button Action(Transform parent, string caption, Vector2 position, Vector2 size,
                UnityAction callback, bool primary = false)
            {
                var node = Panel(parent, ButtonName(caption), Center, position, size,
                    primary ? new Color(.08f, .46f, .58f) : ButtonColor);
                var button = node.gameObject.AddComponent<Button>();
                button.targetGraphic = node.GetComponent<Image>();
                Stretch(Label(node, "Label", caption, primary ? 24 : 18, Vector2.zero, size).rectTransform);
                UnityEventTools.AddPersistentListener(button.onClick, callback);
                return button;
            }

            void BuildInit()
            {
                var panel = Rect(canvas, "Loading", new Vector2(.5f, .26f), Vector2.zero,
                    new Vector2(480, 200));
                var bar = Panel(panel, "Progress", Center, new Vector2(0, 17),
                    new Vector2(438, 16), new Color(.18f, .27f, .38f));
                var fill = Panel(bar, "Fill", Center, Vector2.zero, Vector2.zero, Accent);
                Stretch(fill);
                var slider = bar.gameObject.AddComponent<Slider>();
                slider.fillRect = fill;
                slider.minValue = 0f;
                slider.maxValue = 1f;
                slider.interactable = false;
                slider.transition = Selectable.Transition.None;
                Bind("progress", slider);
                Bind("loadingStatus", Label(panel, "ProgressInfo", "正在加载 0%", 20,
                    new Vector2(0, -14), new Vector2(438, 30)));
                Bind("version", Label(panel, "Version", $"版本号 {Application.version}", 17,
                    new Vector2(0, -47), new Vector2(438, 28)));
                var retry = Action(panel, "重试", new Vector2(0, -80), new Vector2(110, 32), init.RetryLoading);
                Bind("retry", retry);
                retry.gameObject.SetActive(false);
            }

            void BuildGame()
            {
                var menu = Rect(canvas, "MainPanel", Center, Vector2.zero, new Vector2(490, 326));
                Bind("gameMenu", menu.gameObject);

                var row = Rect(menu, "ChannelPanel", Center, new Vector2(0, 54), new Vector2(454, 50));
                var small = new Vector2(102, 48);
                Action(row, "添加桌面", new Vector2(-169, 0), small, main.RequestAddDesktop);
                Action(row, "入口有奖", new Vector2(-56, 0), small, main.RequestEntryReward);
                Action(row, "分享", new Vector2(56, 0), small, main.RequestShare);
                Action(row, "订阅", new Vector2(169, 0), small, main.RequestSubscribe);
                Action(menu, "开始游戏", new Vector2(0, -13), new Vector2(454, 58), main.BeginGame, true);
                Action(menu, "设置", new Vector2(0, -80), new Vector2(454, 42), main.OpenSettings);
                Bind("menuStatus", Label(menu, "Tips", "", 17, new Vector2(0, -129), new Vector2(454, 36)));

                var demo = Panel(canvas, "DemoPanel", new Vector2(0f, .5f), new Vector2(22, 0),
                    new Vector2(360, 410), CardColor);
                Bind("gameplayDemo", demo.gameObject);
                Label(demo, "Title", "玩法演示", 26, new Vector2(0, 161), new Vector2(324, 46));
                var demoSize = new Vector2(324, 43);
                Action(demo, "播放音效", new Vector2(0, 105), demoSize, main.PlaySampleEffect);
                Action(demo, "播放音乐", new Vector2(0, 54), demoSize, main.PlaySampleMusic);
                Action(demo, "停止音乐", new Vector2(0, 3), demoSize, main.StopSampleMusic);
                Action(demo, "租用或归还方块", new Vector2(0, -48), demoSize, main.ToggleSampleCube);
                Bind("demoStatus", Label(demo, "DemoTips", "", 17, new Vector2(0, -99), new Vector2(324, 37)));
                Action(demo, "返回菜单", new Vector2(0, -154), demoSize, main.ReturnToMenu);
                demo.gameObject.SetActive(false);

                var overlay = Panel(canvas, "SettingPanel", Center, Vector2.zero, Vector2.zero,
                    new Color(0, 0, 0, .62f));
                Stretch(overlay);
                var settings = overlay.gameObject.AddComponent<PanelView>();
                Bind("settings", settings);
                var card = Panel(overlay, "Content", Center, Vector2.zero, new Vector2(390, 356), CardColor);
                Label(card, "Title", "设置", 26, new Vector2(0, 130), new Vector2(354, 46));
                var settingSize = new Vector2(354, 43);
                Bind("musicToggleText", Action(card, "音乐", new Vector2(0, 77), settingSize,
                    main.ToggleMusic).GetComponentInChildren<Text>());
                Bind("sfxToggleText", Action(card, "音效", new Vector2(0, 26), settingSize,
                    main.ToggleEffects).GetComponentInChildren<Text>());
                Bind("musicVolumeText", Action(card, "音乐音量", new Vector2(0, -25), settingSize,
                    main.StepMusicVolume).GetComponentInChildren<Text>());
                Bind("sfxVolumeText", Action(card, "音效音量", new Vector2(0, -76), settingSize,
                    main.StepEffectsVolume).GetComponentInChildren<Text>());
                Action(card, "关闭", new Vector2(0, -127), settingSize, settings.Hide);
                overlay.gameObject.SetActive(false);
            }
        }

        internal static void ApplyTemplateArt(RectTransform canvas, bool game)
        {
            var ui = "Assets/GameFrame/Ui/";
            var background = LoadSprite(game ? "com/bjt.png" : "loading/bg.jpg");
            var root = canvas.Find(game ? "MainPanel" : "Loading");
            if (root == null)
                throw new System.InvalidOperationException("Expected a scene UI panel below MainCanvas.");

            var title = root.Find("Title");
            if (title != null) Undo.DestroyObjectImmediate(title.gameObject);
            var cardImage = root.GetComponent<Image>();
            if (cardImage != null) Undo.DestroyObjectImmediate(cardImage);

            var bg = canvas.Find("Bg") as RectTransform;
            if (bg == null)
            {
                bg = Rect(canvas, "Bg", Center, Vector2.zero, Vector2.zero);
                Undo.RegisterCreatedObjectUndo(bg.gameObject, "Add template background");
            }
            var bgImage = bg.GetComponent<Image>() ?? bg.gameObject.AddComponent<Image>();
            UseSprite(bgImage, background, false);
            bgImage.raycastTarget = false;
            bg.SetAsFirstSibling();
            Stretch(bg);
            var fit = bg.GetComponent<AspectRatioFitter>() ?? bg.gameObject.AddComponent<AspectRatioFitter>();
            fit.aspectRatio = background.rect.width / background.rect.height;
            fit.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;

            if (game)
            {
                var topLeft = new Vector2(0f, 1f);
                Stretch((RectTransform)root);
                var setting = (RectTransform)root.Find("SettingBtn");
                Place(setting, topLeft, new Vector2(61, -58), new Vector2(74, 74));
                UseIcon(setting, "com/sz.png");

                var channel = (RectTransform)root.Find("ChannelPanel");
                Place(channel, topLeft, new Vector2(60, -263), new Vector2(72, 300));
                var buttonNames = new[] { "AddDeskBtn", "RewardBtn", "ShareBtn", "SubscribeBtn" };
                var artNames = new[] { "tjzm.png", "rkyj.png", "fx.png", "dy.png" };
                for (var i = 0; i < buttonNames.Length; i++)
                {
                    var button = (RectTransform)channel.Find(buttonNames[i]);
                    Place(button, new Vector2(.5f, 1f), new Vector2(0, -35 - 76 * i), new Vector2(70, 70));
                    UseIcon(button, "com/" + artNames[i]);
                }

                var bottomCenter = new Vector2(.5f, 0f);
                var start = (RectTransform)root.Find("StartBtn");
                Place(start, bottomCenter, new Vector2(0, 133), new Vector2(270, 98));
                UseSprite(start.GetComponent<Image>(), LoadSprite("com/anniu.png"), true);
                var label = start.GetComponentInChildren<Text>(true);
                label.color = new Color(.35f, .17f, .025f);
                label.fontSize = 29;
                var tips = (RectTransform)root.Find("Tips");
                Place(tips, bottomCenter, new Vector2(0, 60), new Vector2(500, 34));
                AddOutline(tips.GetComponent<Text>());
            }
            else
            {
                var loading = (RectTransform)root;
                Place(loading, new Vector2(.5f, .24f), Vector2.zero, new Vector2(650, 195));
                var progress = (RectTransform)loading.Find("Progress");
                Place(progress, Center, new Vector2(0, 12), new Vector2(650, 50));
                var frame = progress.GetComponent<Image>();
                UseSprite(frame, LoadSprite("loading/jdt2.png"), false);
                frame.raycastTarget = false;
                var slider = progress.GetComponent<Slider>();
                var fill = slider.fillRect.GetComponent<Image>();
                UseSprite(fill, LoadSprite("loading/jdt1.png"), false);
                fill.type = Image.Type.Filled;
                fill.fillMethod = Image.FillMethod.Horizontal;
                fill.fillOrigin = (int)Image.OriginHorizontal.Left;
                fill.fillAmount = slider.normalizedValue;
                fill.raycastTarget = false;
                Stretch(slider.fillRect);
                var status = (RectTransform)loading.Find("ProgressInfo");
                Place(status, Center, new Vector2(0, -32), new Vector2(640, 28));
                AddOutline(status.GetComponent<Text>());
                var version = (RectTransform)loading.Find("Version");
                Place(version, Center, new Vector2(0, -60), new Vector2(640, 24));
                AddOutline(version.GetComponent<Text>());
                var retry = (RectTransform)loading.Find("RetryBtn");
                Place(retry, Center, new Vector2(0, -101), new Vector2(135, 49));
                UseSprite(retry.GetComponent<Image>(), LoadSprite("com/anniu.png"), true);
                retry.GetComponentInChildren<Text>(true).color = new Color(.35f, .17f, .025f);
            }

            Sprite LoadSprite(string name)
            {
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(ui + name);
                if (sprite == null)
                    throw new System.InvalidOperationException("Missing UI sprite: " + ui + name);
                return sprite;
            }

            void UseIcon(RectTransform button, string path)
            {
                UseSprite(button.GetComponent<Image>(), LoadSprite(path), true);
                // The reused artwork includes the Chinese caption.
                button.GetComponentInChildren<Text>(true).enabled = false;
            }
        }

        private static void UseSprite(Image image, Sprite sprite, bool preserveAspect)
        {
            image.sprite = sprite;
            image.type = Image.Type.Simple;
            image.color = Color.white;
            image.preserveAspect = preserveAspect;
        }

        private static void Place(RectTransform rect, Vector2 anchor, Vector2 position, Vector2 size)
        {
            rect.anchorMin = rect.anchorMax = anchor;
            rect.pivot = Center;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }

        private static void AddOutline(Text text)
        {
            var outline = text.GetComponent<Outline>() ?? text.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0f, 0f, 0f, .9f);
            outline.effectDistance = new Vector2(1f, -1f);
        }

        private static string ButtonName(string caption) => caption switch
        {
            "设置" => "SettingBtn",
            "开始游戏" => "StartBtn",
            "添加桌面" => "AddDeskBtn",
            "入口有奖" => "RewardBtn",
            "分享" => "ShareBtn",
            "订阅" => "SubscribeBtn",
            "关闭" => "CloseBtn",
            "重试" => "RetryBtn",
            "返回菜单" => "BackBtn",
            "播放音效" => "PlaySfxBtn",
            "播放音乐" => "PlayMusicBtn",
            "停止音乐" => "StopMusicBtn",
            "租用或归还方块" => "PoolBtn",
            "音乐" => "MusicBtn",
            "音效" => "SfxBtn",
            "音乐音量" => "MusicVolumeBtn",
            "音效音量" => "SfxVolumeBtn",
            _ => caption
        };

        private static RectTransform Panel(Transform parent, string name, Vector2 anchor,
            Vector2 position, Vector2 size, Color color)
        {
            var rect = Rect(parent, name, anchor, position, size);
            rect.gameObject.AddComponent<Image>().color = color;
            return rect;
        }

        private static RectTransform Rect(Transform parent, string name, Vector2 anchor, Vector2 position, Vector2 size)
        {
            var node = new GameObject(name, typeof(RectTransform));
            node.layer = LayerMask.NameToLayer("UI");
            var rect = (RectTransform)node.transform;
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = anchor;
            rect.pivot = new Vector2(anchor.x == 0f ? 0f : .5f, .5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            return rect;
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
        }
    }
}
