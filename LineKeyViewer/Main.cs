using System;
using System.Threading;
using JALib.Core;
using JALib.Tools;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace LineKeyViewer;

public class Main() : JAMod(typeof(Setting)) {
    public static Main Instance;
    public static SettingGUI SettingGUI;
    public static new Setting Setting;
    public static GameObject KeyViewerObject;
    public static RectTransform SizeTransform;
    public static RectTransform LocationTransform;
    public static AsyncImage MainImage;
    public static AsyncImage LeftHand;
    public static AsyncImage RightHand;
    public static AsyncImage Head;
    public static Key[] Keys;
    public static bool HeadOn;
    public static bool WinkOn;
    public static bool GameResult;
    private static string SizeString;
    private static string LocationXString;
    private static string LocationYString;
    private static int SelectedKey;
    private static int WinAPICool;
    private static bool[] KeyPressed;
    private static Thread[] threads;

    protected override void OnSetup() {
        Setting = (Setting) base.Setting;
        SettingGUI = new SettingGUI(this);
        SelectedKey = -1;
        Patcher.AddPatch(typeof(ResultHandler));
    }

    protected override void OnEnable() {
        ModEntry.Info.DisplayName = ModEntry.Info.Id;
        BundleManager bundleManager = BundleManager.Instance = new BundleManager();
        Setting setting = Setting;
        Keys = new Key[16];
        KeyViewerObject = new GameObject("LineKeyViewer");
        Canvas canvas = KeyViewerObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvas.gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        canvas.gameObject.AddComponent<GraphicRaycaster>();
        
        GameObject gameObj = new("LocationObject");
        RectTransform rectTransform = LocationTransform = gameObj.AddComponent<RectTransform>();
        rectTransform.SetParent(KeyViewerObject.transform);
        rectTransform.anchorMin = rectTransform.anchorMax = Vector2.zero;
        UpdateLocation();
        
        gameObj = new GameObject("SizeObject");
        rectTransform = SizeTransform = gameObj.AddComponent<RectTransform>();
        rectTransform.SetParent(LocationTransform);
        rectTransform.sizeDelta = new Vector2(540, 420);
        rectTransform.localScale = new Vector3(Setting.Size, Setting.Size);
        rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.offsetMin = rectTransform.offsetMax = Vector2.zero;
        
        gameObj = new GameObject("Main");
        rectTransform = gameObj.AddComponent<RectTransform>();
        rectTransform.SetParent(SizeTransform);
        rectTransform.sizeDelta = new Vector2(540, 420);
        rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = rectTransform.anchoredPosition = Vector2.zero;
        (MainImage = gameObj.AddComponent<AsyncImage>()).image.sprite = setting.HideDesk ? bundleManager.Line : bundleManager.LineTable;

        gameObj = new GameObject("piano");
        rectTransform = gameObj.AddComponent<RectTransform>();
        rectTransform.SetParent(SizeTransform);
        rectTransform.sizeDelta = new Vector2(540, 420);
        rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = rectTransform.anchoredPosition = Vector2.zero;
        Image image = gameObj.AddComponent<Image>();
        image.sprite = bundleManager.Piano;
        image.type = Image.Type.Sliced;

        gameObj = new GameObject("Key1");
        rectTransform = gameObj.AddComponent<RectTransform>();
        rectTransform.SetParent(SizeTransform);
        rectTransform.sizeDelta = new Vector2(61, 39);
        rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = Vector2.zero;
        rectTransform.anchoredPosition = new Vector2(117, 149);
        (Keys[0] = gameObj.AddComponent<Key>()).image.sprite = bundleManager.KeySprites[0];
        
        gameObj = new GameObject("Key2");
        rectTransform = gameObj.AddComponent<RectTransform>();
        rectTransform.SetParent(SizeTransform);
        rectTransform.sizeDelta = new Vector2(56, 40);
        rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = Vector2.zero;
        rectTransform.anchoredPosition = new Vector2(151, 141);
        (Keys[1] = gameObj.AddComponent<Key>()).image.sprite = bundleManager.KeySprites[1];
        
        gameObj = new GameObject("Key3");
        rectTransform = gameObj.AddComponent<RectTransform>();
        rectTransform.SetParent(SizeTransform);
        rectTransform.sizeDelta = new Vector2(52, 41);
        rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = Vector2.zero;
        rectTransform.anchoredPosition = new Vector2(186, 133);
        (Keys[2] = gameObj.AddComponent<Key>()).image.sprite = bundleManager.KeySprites[2];
        
        gameObj = new GameObject("Key4");
        rectTransform = gameObj.AddComponent<RectTransform>();
        rectTransform.SetParent(SizeTransform);
        rectTransform.sizeDelta = new Vector2(50, 39);
        rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = Vector2.zero;
        rectTransform.anchoredPosition = new Vector2(217, 127);
        (Keys[3] = gameObj.AddComponent<Key>()).image.sprite = bundleManager.KeySprites[3];
        
        gameObj = new GameObject("Key5");
        rectTransform = gameObj.AddComponent<RectTransform>();
        rectTransform.SetParent(SizeTransform);
        rectTransform.sizeDelta = new Vector2(50, 39);
        rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = Vector2.zero;
        rectTransform.anchoredPosition = new Vector2(247, 119);
        (Keys[4] = gameObj.AddComponent<Key>()).image.sprite = bundleManager.KeySprites[4];
        
        gameObj = new GameObject("Key6");
        rectTransform = gameObj.AddComponent<RectTransform>();
        rectTransform.SetParent(SizeTransform);
        rectTransform.sizeDelta = new Vector2(49, 39);
        rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = Vector2.zero;
        rectTransform.anchoredPosition = new Vector2(277, 112);
        (Keys[5] = gameObj.AddComponent<Key>()).image.sprite = bundleManager.KeySprites[5];
        
        gameObj = new GameObject("Key7");
        rectTransform = gameObj.AddComponent<RectTransform>();
        rectTransform.SetParent(SizeTransform);
        rectTransform.sizeDelta = new Vector2(47, 39);
        rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot =Vector2.zero;
        rectTransform.anchoredPosition = new Vector2(307, 106);
        (Keys[6] = gameObj.AddComponent<Key>()).image.sprite = bundleManager.KeySprites[6];
        
        gameObj = new GameObject("Key8");
        rectTransform = gameObj.AddComponent<RectTransform>();
        rectTransform.SetParent(SizeTransform);
        rectTransform.sizeDelta = new Vector2(46, 38);
        rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = Vector2.zero;
        rectTransform.anchoredPosition = new Vector2(336, 99);
        (Keys[7] = gameObj.AddComponent<Key>()).image.sprite = bundleManager.KeySprites[7];
        
        gameObj = new GameObject("Key9");
        rectTransform = gameObj.AddComponent<RectTransform>();
        rectTransform.SetParent(SizeTransform);
        rectTransform.sizeDelta = new Vector2(39, 22);
        rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = Vector2.zero;
        rectTransform.anchoredPosition = new Vector2(158, 183);
        (Keys[8] = gameObj.AddComponent<Key>()).image.sprite = bundleManager.KeySprites[8];
        
        gameObj = new GameObject("Key10");
        rectTransform = gameObj.AddComponent<RectTransform>();
        rectTransform.SetParent(SizeTransform);
        rectTransform.sizeDelta = new Vector2(40, 23);
        rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = Vector2.zero;
        rectTransform.anchoredPosition = new Vector2(185, 176);
        (Keys[9] = gameObj.AddComponent<Key>()).image.sprite = bundleManager.KeySprites[9];
        
        gameObj = new GameObject("Key11");
        rectTransform = gameObj.AddComponent<RectTransform>();
        rectTransform.SetParent(SizeTransform);
        rectTransform.sizeDelta = new Vector2(40, 24);
        rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = Vector2.zero;
        rectTransform.anchoredPosition = new Vector2(215, 169);
        (Keys[10] = gameObj.AddComponent<Key>()).image.sprite = bundleManager.KeySprites[10];
        
        gameObj = new GameObject("Key12");
        rectTransform = gameObj.AddComponent<RectTransform>();
        rectTransform.SetParent(SizeTransform);
        rectTransform.sizeDelta = new Vector2(39, 25);
        rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = Vector2.zero;
        rectTransform.anchoredPosition = new Vector2(244, 161);
        (Keys[11] = gameObj.AddComponent<Key>()).image.sprite = bundleManager.KeySprites[11];
        
        gameObj = new GameObject("Key13");
        rectTransform = gameObj.AddComponent<RectTransform>();
        rectTransform.SetParent(SizeTransform);
        rectTransform.sizeDelta = new Vector2(38, 26);
        rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = Vector2.zero;
        rectTransform.anchoredPosition = new Vector2(273, 154);
        (Keys[12] = gameObj.AddComponent<Key>()).image.sprite = bundleManager.KeySprites[12];
        
        gameObj = new GameObject("Key14");
        rectTransform = gameObj.AddComponent<RectTransform>();
        rectTransform.SetParent(SizeTransform);
        rectTransform.sizeDelta = new Vector2(39, 26);
        rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = Vector2.zero;
        rectTransform.anchoredPosition = new Vector2(301, 147);
        (Keys[13] = gameObj.AddComponent<Key>()).image.sprite = bundleManager.KeySprites[13];
        
        gameObj = new GameObject("Key15");
        rectTransform = gameObj.AddComponent<RectTransform>();
        rectTransform.SetParent(SizeTransform);
        rectTransform.sizeDelta = new Vector2(40, 26);
        rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = Vector2.zero;
        rectTransform.anchoredPosition = new Vector2(330, 140);
        (Keys[14] = gameObj.AddComponent<Key>()).image.sprite = bundleManager.KeySprites[14];
        
        gameObj = new GameObject("Key16");
        rectTransform = gameObj.AddComponent<RectTransform>();
        rectTransform.SetParent(SizeTransform);
        rectTransform.sizeDelta = new Vector2(39, 26);
        rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = Vector2.zero;
        rectTransform.anchoredPosition = new Vector2(359, 133);
        (Keys[15] = gameObj.AddComponent<Key>()).image.sprite = bundleManager.KeySprites[15];
        
        gameObj = new GameObject("RightHand");
        rectTransform = gameObj.AddComponent<RectTransform>();
        rectTransform.SetParent(SizeTransform);
        rectTransform.sizeDelta = new Vector2(270, 420);
        rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = rectTransform.anchoredPosition = Vector2.zero;
        (RightHand = gameObj.AddComponent<AsyncImage>()).image.sprite = bundleManager.UnpressedKeySprites[1];
        
        gameObj = new GameObject("LeftHand");
        rectTransform = gameObj.AddComponent<RectTransform>();
        rectTransform.SetParent(SizeTransform);
        rectTransform.sizeDelta = new Vector2(270, 420);
        rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = Vector2.right;
        rectTransform.anchoredPosition = new Vector2(540, 0);
        (LeftHand = gameObj.AddComponent<AsyncImage>()).image.sprite = bundleManager.UnpressedKeySprites[0];
        
        gameObj = new GameObject("Head");
        rectTransform = gameObj.AddComponent<RectTransform>();
        rectTransform.SetParent(SizeTransform);
        rectTransform.sizeDelta = new Vector2(540, 420);
        rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = rectTransform.anchoredPosition = Vector2.zero;
        image = (Head = gameObj.AddComponent<AsyncImage>()).image;
        image.sprite = bundleManager.LineHead;
        image.enabled = false;
        
        if(setting.FlipHorizontal) SizeTransform.eulerAngles = new Vector3(0, 180, 0);
        Object.DontDestroyOnLoad(KeyViewerObject);
        
        threads = new Thread[2];
        (threads[0] = new Thread(KeyInputManager.ListenKey)).Start();
        (threads[1] = new Thread(Winking)).Start();
        Application.quitting += OnQuitting;
    }

    private static void OnQuitting() {
        foreach(Thread thread in threads) thread.Abort();
    }

    protected override void OnGUI() {
        Setting setting = Setting;
        SettingGUI settingGUI = SettingGUI;
        JALocalization localization = Localization;
        settingGUI.AddSettingSliderFloat(ref setting.Size, 1, ref SizeString, localization["size"], 0, 2, () => {
            SizeTransform.localScale = new Vector3(setting.Size, setting.Size);
            UpdateLocation();
        });
        settingGUI.AddSettingSliderFloat(ref setting.LocationX, 0, ref LocationXString, localization["locationX"], 0, 1, UpdateLocation);
        settingGUI.AddSettingSliderFloat(ref setting.LocationY, 0, ref LocationYString, localization["locationY"], 0, 1, UpdateLocation);
        settingGUI.AddSettingToggle(ref setting.FlipHorizontal, localization["flipHorizontal"], () => {
            SizeTransform.eulerAngles = new Vector3(0, setting.FlipHorizontal ? 180 : 0, 0);
            KeyInputManager.Reset = true;
            UpdateLocation();
        });
        settingGUI.AddSettingToggle(ref setting.HideDesk, localization["hideDesk"], () => {
            if(HeadOn) {
                if(setting.HideDesk) MainImage.enable = 0;
                else {
                    MainImage.enable = 1;
                    MainImage.sprite = BundleManager.Instance.Table;
                }
            } else MainImage.sprite = WinkOn ?
                                          setting.HideDesk ? BundleManager.Instance.LineWink : BundleManager.Instance.LineWinkTable :
                                          setting.HideDesk ? BundleManager.Instance.Line : BundleManager.Instance.LineTable;
            
        });
        if(JipperResourcePackAPI.CheckJipperResourcePack()) settingGUI.AddSettingToggle(ref setting.ShareJipperResourcePack, localization["shareJipperResourcePack"], () => {
            setting.ShareJipperKeyCode(setting.ShareJipperResourcePack);
        });
        GUILayout.Space(18f);
        GUILayout.BeginHorizontal();
        for(int i = 0; i < 8; i++) CreateButton(i);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        for(int i = 8; i < 16; i++) CreateButton(KeyInputManager.Location[i]);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        if(SelectedKey == -1) return;
        GUILayout.Label($"<b>{localization["inputKey"]}</b>");
        if(Input.anyKeyDown) {
            foreach(KeyCode keyCode in Enum.GetValues(typeof(KeyCode))) {
                if(!Input.GetKeyDown(keyCode)) continue;
                SetupKey(keyCode);
                break;
            }
        } else {
            for(int i = 0; i < 256; i++) {
                if((KeyInputManager.GetAsyncKeyState(i) & 0x8000) != 0 == KeyPressed[i]) continue;
                if(KeyPressed[i]) {
                    KeyPressed[i] = false;
                    WinAPICool = 0;
                    continue;
                }
                if(WinAPICool++ < 6) break;
                KeyCode keyCode = (KeyCode) i + 0x1000;
                SetupKey(keyCode);
                break;
            }
        }
    }
    
    private void CreateButton(int i) {
        if(!GUILayout.Button(Bold(KeyToString(Setting.KeyCodes[i]), i == SelectedKey))) return;
        SelectedKey = i;
        WinAPICool = 0;
        KeyPressed = new bool[256];
        for(int i2 = 0; i2 < 256; i2++) KeyPressed[i2] = (KeyInputManager.GetAsyncKeyState(i2) & 0x8000) != 0;
    }
    
    private void SetupKey(KeyCode keyCode) {
        Setting.KeyCodes[SelectedKey] = keyCode;
        SelectedKey = -1;
        WinAPICool = 0;
        KeyPressed = null;
        SaveSetting();
        if(!Setting.KeyCodeJipperResourcePack) return;
        JipperResourcePackAPI.UpdateKeyLimit();
        JipperResourcePackAPI.SaveSetting();
    }
    
    private static string Bold(string text, bool bold) => !bold ? text : $"<b>{text}</b>";

    private static string KeyToString(KeyCode keyCode) => (int) keyCode < 0x1000 ? keyCode.ToString() : ((System.Windows.Forms.Keys) (keyCode - 0x1000)).ToString();
    
    private static void UpdateLocation() {
        Setting setting = Setting;
        float y = 1 - setting.LocationY;
        RectTransform rectTransform = LocationTransform;
        rectTransform.sizeDelta = new Vector2(540 * setting.Size, 420 * setting.Size);
        rectTransform.pivot = new Vector2(setting.LocationX, y);
        rectTransform.anchoredPosition = new Vector2(setting.LocationX * 1920 + (setting.FlipHorizontal ? 540 : 0), y * 1080);
    }

    private void Winking() {
        try {
            while(Enabled) {
                do {
                    Thread.Sleep(JARandom.Instance.Next(3000, 7000));
                } while(HeadOn || GameResult);
                WinkOn = true;
                MainImage.sprite = Setting.HideDesk ? BundleManager.Instance.LineWink : BundleManager.Instance.LineWinkTable;
                Thread.Sleep(JARandom.Instance.Next(100, 250));
                if(!WinkOn) continue;
                WinkOn = false;
                MainImage.sprite = Setting.HideDesk ? BundleManager.Instance.Line : BundleManager.Instance.LineTable;
            }
        } catch (ThreadAbortException) {
        } catch (Exception e) {
            if(!Enabled) return;
            LogReportException("Failed to work wink", e);
        }
    }

    protected override void OnDisable() {
        if(KeyViewerObject) {
            Object.Destroy(KeyViewerObject);
            KeyViewerObject = null;
            SizeTransform = null;
            Keys = null;
        }
        foreach(Thread thread in threads) thread.Abort();
        threads = null;
        BundleManager.Instance.Dispose();
    }
}