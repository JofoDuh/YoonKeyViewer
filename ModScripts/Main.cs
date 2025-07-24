using HarmonyLib;
using JipperResourcePack.Keyviewer;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;
using System.IO;
using YoonKeyViewer.Component;

#if UNITYMODMANAGER
using UnityModManagerNet;

// Jongyeol if you are reading this, excuse my bad code :p
namespace YoonKeyViewer
{
    public static class Main
    {
        public static bool IsEnabled = false;
        public static UnityModManager.ModEntry.ModLogger Logger;
        public static UnityModManager.ModEntry ModEntry;
        public static Harmony harmony;
        public static Setting setting;
        public static scrYoonKeyViewer KeyViewer;
        private static int SelectedKey;
        private static int WinAPICool;
        private static bool[] KeyPressed = null;
        private static Thread[] threads;
        public static string ModPath;

        internal static void Setup(UnityModManager.ModEntry modEntry)
        {
            setting = Setting.Load(modEntry);
            ModEntry = modEntry;
            Logger = modEntry.Logger;
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            ModPath = modEntry.Path;
        }

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            IsEnabled = value;
            if (value)
            {
                harmony = new Harmony(modEntry.Info.Id);
                harmony.PatchAll(Assembly.GetExecutingAssembly());
                YoonBundleManager bundleManager = YoonBundleManager.Instance = new YoonBundleManager();
                GameObject gameObject = UnityEngine.Object.Instantiate(bundleManager.KeyViewerObject);
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                KeyViewer = gameObject.GetComponent<scrYoonKeyViewer>();
                KeyViewer.sizeTransform.localScale = new Vector3(setting.Size, setting.Size);
                if (setting.FlipHorizontal) KeyViewer.sizeTransform.eulerAngles = new Vector3(0, 180, 0);
                if (setting.HideDesk) KeyViewer.Table.enable = 1;
                UpdateLocation();

                threads = new Thread[2];
                (threads[0] = new Thread(KeyInputManager.ListenKey)).Start();
                (threads[1] = new Thread(Winking)).Start();

                Application.quitting += OnQuitting;
            }
            else
            {
                if (KeyViewer)
                {
                    UnityEngine.Object.Destroy(KeyViewer.gameObject);
                    KeyViewer = null;
                }
                foreach (Thread thread in threads) thread.Abort();
                threads = null;
                YoonBundleManager.Instance.Dispose();
                harmony.UnpatchAll(modEntry.Info.Id);
            }
            return true;
        }

        private static void OnQuitting()
        {
            foreach (Thread thread in threads) thread.Abort();
        }
        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.Label($"{setting.GetLocalized("ykv.settingslabel")}");
            float newSize = NamedSlider("Size:", setting.Size, 0, 2, 150);
            if (newSize != setting.Size)
            {
                setting.Size = newSize;
                KeyViewer.sizeTransform.localScale = new Vector3(setting.Size, setting.Size);
                UpdateLocation();
            }
            float newLocationX = NamedSlider($"{setting.GetLocalized("ykv.posx")}:", setting.LocationX, 0, 1, 250);
            if (newLocationX != setting.LocationX)
            {
                setting.LocationX = newLocationX;
                UpdateLocation();
            }
            float newLocationY = NamedSlider($"{setting.GetLocalized("ykv.posy")}:", setting.LocationY, 0, 1, 250);
            if (newLocationY != setting.LocationY)
            {
                setting.LocationY = newLocationY;
                UpdateLocation();
            }
            float newNervousBPM = NamedSlider($"{setting.GetLocalized("ykv.nervousbpm")}:", setting.NervousBPM, 1, 1000, 250);
            newNervousBPM = Mathf.Max(1, newNervousBPM); // Clamp to minimum of 1
            if (newNervousBPM != setting.NervousBPM)
            {
                setting.NervousBPM = newNervousBPM;
            }
            bool newFlipHorizontal = GUILayout.Toggle(setting.FlipHorizontal, $"{setting.GetLocalized("ykv.fliphorizontal")}:");
            if (newFlipHorizontal != setting.FlipHorizontal)
            {
                setting.FlipHorizontal = newFlipHorizontal;
                KeyViewer.sizeTransform.eulerAngles = new Vector3(0, setting.FlipHorizontal ? 180 : 0, 0);
                KeyInputManager.Reset = true;
            }
            bool newHideDesk = GUILayout.Toggle(setting.HideDesk, $"{setting.GetLocalized("ykv.hidedesk")}:");
            if (newHideDesk !=  setting.HideDesk)
            {
                setting.HideDesk = newHideDesk;
                scrYoonKeyViewer keyViewer = KeyViewer;
                if (keyViewer.isSmashing)
                {
                    if (setting.HideDesk) keyViewer.Table.enable = 0;
                    else
                    {
                        keyViewer.Table.enable = 1;
                    }
                }
                else
                {
                    if (setting.HideDesk) keyViewer.Table.enable = 0;
                    else
                    {
                        keyViewer.Table.enable = 1;
                    }
                }
            }

            bool newHideLegs = GUILayout.Toggle(setting.HideFeet, $"{setting.GetLocalized("ykv.hidelegs")}:");
            if (newHideLegs !=  setting.HideFeet)
            {
                setting.HideFeet = newHideLegs;
            }
            if (!setting.HideFeet)
            {
                bool newHideFeetKeyboard = GUILayout.Toggle(setting.HideFeetKeyboard, $"{setting.GetLocalized("ykv.hidefeetkeyboard")}");
                if (newHideFeetKeyboard != setting.HideFeetKeyboard)
                {
                    setting.HideFeetKeyboard = newHideFeetKeyboard;
                }
            }
            
            // Top Row
            GUILayout.Space(18f);
            GUILayout.Label(setting.GetLocalized("ykv.toprow"));
            GUILayout.BeginHorizontal();
            for (int i = 0; i < 8; i++) CreateButton(i);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            // Bottom Row
            GUILayout.Label(setting.GetLocalized("ykv.bottomrow"));
            GUILayout.BeginHorizontal();
            for (int i = 8; i < 16; i++) CreateButton(KeyInputManager.HandLocation[i]);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            // Feet Keyboard
            if (!setting.HideFeet && !setting.HideFeetKeyboard)
            {
                GUILayout.Label(setting.GetLocalized("ykv.feetkeyboard"));
                GUILayout.BeginHorizontal();
                for (int i = 0; i < 4; i++) CreateFootButton(i);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            if (SelectedKey == -1 || KeyPressed == null) return;
            GUILayout.Label($"<b>{setting.GetLocalized("ykv.pressakey")}</b>");
            if (Input.anyKeyDown)
            {
                foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
                {
                    if (!Input.GetKeyDown(keyCode)) continue;
                    SetupKey(keyCode);
                    break;
                }
            }
            else
            {
                for (int i = 0; i < 256; i++)
                {
                    if ((KeyInputManager.GetAsyncKeyState(i) & 0x8000) != 0 == KeyPressed[i]) continue;
                    if (KeyPressed[i])
                    {
                        KeyPressed[i] = false;
                        WinAPICool = 0;
                        continue;
                    }
                    if (WinAPICool++ < 6) break;
                    KeyCode keyCode = (KeyCode)i + 0x1000;
                    SetupKey(keyCode);
                    break;
                }
            }
        }

        private static void CreateButton(int i)
        {
            if (!GUILayout.Button(Bold(KeyToString(setting.KeyCodes[i]), i == SelectedKey))) return;
            SelectedKey = i;
            WinAPICool = 0;
            KeyPressed = new bool[256];
            for (int i2 = 0; i2 < 256; i2++) KeyPressed[i2] = (KeyInputManager.GetAsyncKeyState(i2) & 0x8000) != 0;
        }
        private static void CreateFootButton(int i)
        {
            if (!GUILayout.Button(Bold(KeyToString(setting.FKeyCodes[i]), i + 100 == SelectedKey))) return;
            SelectedKey = i + 100; // Offset to distinguish from hand keys
            WinAPICool = 0;
            KeyPressed = new bool[256];
            for (int i2 = 0; i2 < 256; i2++)
                KeyPressed[i2] = (KeyInputManager.GetAsyncKeyState(i2) & 0x8000) != 0;
        }

        private static void SetupKey(KeyCode keyCode)
        {
            if (SelectedKey >= 100)
            {
                setting.FKeyCodes[SelectedKey - 100] = keyCode;
            }
            else
            {
                setting.KeyCodes[SelectedKey] = keyCode;
            }

            SelectedKey = -4;
            WinAPICool = 0;
            KeyPressed = null;
            setting.Save(ModEntry);
        }

        private static string Bold(string text, bool bold) => !bold ? text : $"<b>{text}</b>";

        private static string KeyToString(KeyCode keyCode) => (int)keyCode < 0x1000 ? keyCode.ToString() : ((System.Windows.Forms.Keys)(keyCode - 0x1000)).ToString();

        private static void UpdateLocation()
        {
            float y = 1 - setting.LocationY;
            RectTransform rectTransform = KeyViewer.locationTransform;
            rectTransform.sizeDelta = new Vector2(540 * setting.Size, 420 * setting.Size);
            rectTransform.pivot = new Vector2(setting.LocationX, y);
            rectTransform.anchoredPosition = new Vector2(setting.LocationX * 1920, y * 1080);
        }

        private static void Winking()
        {
            try
            {
                while (IsEnabled)
                {
                    do
                    {
                        Thread.Sleep(UnityEngine.Random.Range(3000, 7000));
                    } while (KeyViewer.isSmashing || KeyViewer.gameResult || KeyViewer.isNervous);
                    KeyViewer.winkOn = true;
                    KeyViewer.Table.enable = setting.HideDesk ? (sbyte)0: (sbyte)1;
                    KeyViewer.Yoon.sprite = YoonBundleManager.Instance.YoonIdleWink;
                    KeyViewer.YoonSmash.enable = 0;
                    KeyViewer.YoonClear.enable = 0;
                    //KeyViewer.mainImage.sprite = setting.HideDesk ? LineBundleManager.Instance.LineWink : LineBundleManager.Instance.LineWinkTable;
                    Thread.Sleep(UnityEngine.Random.Range(100, 250));
                    if (!KeyViewer.winkOn) continue;
                    KeyViewer.winkOn = false;
                    KeyViewer.Yoon.sprite = YoonBundleManager.Instance.YoonIdle;
                    KeyViewer.YoonSmash.enable = 0;
                    KeyViewer.YoonClear.enable = 0;
                    KeyViewer.Table.enable = setting.HideDesk ? (sbyte)0 : (sbyte)1;
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception e)
            {
                if (!IsEnabled) return;
                Logger.Log("[Exception] Failed to work wink: " + e);
            }
        }
        public static float NamedSlider(string name, float value, float leftValue, float rightValue, float sliderWidth,
    float roundNearest = 0, float labelWidth = 0, string valueFormat = "{0}", string endformat = "", float inputFieldWidth = 60f,
    bool useInputFieldAsLabel = true)
        {
            GUILayout.BeginHorizontal();
            float newValue = NamedSliderContent(
                name,
                value,
                leftValue,
                rightValue,
                sliderWidth,
                roundNearest,
                labelWidth,
                valueFormat,
                endformat,
                inputFieldWidth,
                useInputFieldAsLabel
            );
            GUILayout.EndHorizontal();
            return newValue;
        }

        private static float NamedSliderContent(string name, float value, float leftValue, float rightValue, float sliderWidth,
    float roundNearest = 0, float labelWidth = 0, string valueFormat = "{0}", string endformat = "", float inputFieldWidth = 60f,
    bool useInputFieldAsLabel = true)
        {
            if (labelWidth == 0)
            {
                GUILayout.Label(name);
            }
            else
            {
                GUILayout.Label(name, GUILayout.Width(labelWidth));
            }

            GUILayout.Space(4f);

            // Slider
            float newValue = GUILayout.HorizontalSlider(value, leftValue, rightValue, GUILayout.Width(sliderWidth));
            if (roundNearest != 0)
            {
                newValue = Mathf.Round(newValue / roundNearest) * roundNearest;
            }

            GUILayout.Space(8f);

            if (useInputFieldAsLabel)
            {
                // Input field for exact numeric value
                string valueStr = GUILayout.TextField(value.ToString("F2"), GUILayout.Width(inputFieldWidth));

                // Only process if the text has actually changed
                if (valueStr != value.ToString("F2"))
                {
                    // Check for valid float string (allowing negative and decimal)
                    if (string.IsNullOrWhiteSpace(valueStr))
                    {
                        newValue = value; // Reset to previous value if empty
                    }
                    else if (IsValidFloatString(valueStr) && float.TryParse(valueStr, out float parsed))
                    {
                        newValue = parsed;
                        if (roundNearest != 0)
                        {
                            newValue = Mathf.Round(newValue / roundNearest) * roundNearest;
                        }
                    }
                    else
                    {
                        newValue = value; // Reset to previous value if invalid
                    }
                }

                if (!string.IsNullOrEmpty(endformat))
                {
                    GUILayout.Space(4f);
                    GUILayout.Label(endformat);
                }
            }
            else
            {
                GUILayout.Label(string.Format(valueFormat, newValue) + endformat, GUILayout.Width(40f));
            }

            GUILayout.FlexibleSpace();
            return newValue;
        }

        // Helper method to check if a string could be a valid float (allowing negative and decimal)
        private static bool IsValidFloatString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            // Check for multiple decimal points or minus signs
            if (input.Count(c => c == '.') > 1 || input.Count(c => c == '-') > 1)
                return false;

            // Check if minus is in the correct position
            if (input.Contains('-') && input.IndexOf('-') != 0)
                return false;

            // Check if all characters are digits, minus, or decimal point
            foreach (char c in input)
            {
                if (!char.IsDigit(c) && c != '.' && c != '-')
                    return false;
            }

            return true;
        }
        private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            setting.Save(modEntry);
        }
    }
}
#endif

#if BEPINEX
using BepInEx;
using BepInEx.Logging;
using System.Diagnostics.Eventing.Reader;

namespace YoonKeyViewer
{
    public static class YoonKeyViewerInfo
    {
        public const string PLUGIN_GUID = "BepInEx5.YoonKeyViewer";
        public const string PLUGIN_NAME = "YoonKeyViewer";
        public const string PLUGIN_VERSION = "1.0.0";
    }

    // Keep this so you can still use Logger.Log(...) instead of Logger.LogInfo(...)
    public static class LoggerExtensions
    {
        public static void Log(this ManualLogSource logger, string message)
        {
            logger.LogInfo(message);
        }
    }


    [BepInPlugin(YoonKeyViewerInfo.PLUGIN_GUID, YoonKeyViewerInfo.PLUGIN_NAME, YoonKeyViewerInfo.PLUGIN_VERSION)]
    public class Main : BaseUnityPlugin
    {

        public static Main Instance;
        public static ManualLogSource Logger;
        public static string ModPath => Path.Combine(Paths.PluginPath, "YoonKeyViewer");
        public static Setting setting;
        public static scrYoonKeyViewer KeyViewer;
        public static YoonBundleManager BundleManager;
        public static bool IsEnabled = false;

        private static Thread inputThread;
        private static Thread winkThread;
        private Harmony harmony;

        private void Awake()
        {
            Instance = this;
            Logger = base.Logger;
            Logger.LogInfo("YoonKeyViewer Awake");
            // Init settings
            setting = new Setting(); // You may need to pass the correct path
                                     // Init bundle manager
            BundleManager = YoonBundleManager.Instance = new YoonBundleManager();

            // Load viewer
            GameObject obj = Instantiate(BundleManager.KeyViewerObject);
            DontDestroyOnLoad(obj);
            KeyViewer = obj.GetComponent<scrYoonKeyViewer>();

            ApplySettings();

            // Start input & wink threads
            inputThread = new Thread(KeyInputManager.ListenKey) { IsBackground = true };
            winkThread = new Thread(Winking) { IsBackground = true };
            inputThread.Start();
            winkThread.Start();

            Application.quitting += OnQuitting;

            // Attach GUI
            GameObject guiObj = new GameObject("YoonKeyViewerGUI");
            //guiObj.AddComponent<YoonKeyViewerGUI>();
            DontDestroyOnLoad(guiObj);
            IsEnabled = true;

            // Init Harmony patches
            harmony = new Harmony(YoonKeyViewerInfo.PLUGIN_GUID);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

        }

        private void ApplySettings()
        {
            KeyViewer.sizeTransform.localScale = new Vector3(setting.Size, setting.Size);
            KeyViewer.sizeTransform.eulerAngles = new Vector3(0, setting.FlipHorizontal ? 180 : 0, 0);

            if (setting.HideDesk)
                KeyViewer.Table.enable = 0;

            UpdateLocation();
        }

        public void UpdateLocation()
        {
            float y = 1f - setting.LocationY;
            RectTransform rt = KeyViewer.locationTransform;
            rt.sizeDelta = new Vector2(540 * setting.Size, 420 * setting.Size);
            rt.pivot = new Vector2(setting.LocationX, y);
            rt.anchoredPosition = new Vector2(setting.LocationX * 1920, y * 1080);
        }

        private void Winking()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(UnityEngine.Random.Range(3000, 7000));

                    if (KeyViewer.isSmashing || KeyViewer.gameResult || KeyViewer.isNervous)
                        continue;

                    KeyViewer.winkOn = true;
                    KeyViewer.Yoon.sprite = BundleManager.YoonIdleWink;
                    KeyViewer.YoonSmash.enable = 0;
                    KeyViewer.YoonClear.enable = 0;
                    KeyViewer.Table.enable = setting.HideDesk ? (sbyte)0 : (sbyte)1;

                    Thread.Sleep(UnityEngine.Random.Range(100, 250));
                    if (!KeyViewer.winkOn) continue;

                    KeyViewer.winkOn = false;
                    KeyViewer.Yoon.sprite = BundleManager.YoonIdle;
                    KeyViewer.Table.enable = setting.HideDesk ? (sbyte)0 : (sbyte)1;
                }
            }
            catch (ThreadAbortException)
            {
                // expected on quit
            }
            catch (Exception e)
            {
                Logger.LogError($"[Winking] Exception: {e}");
            }
        }

        private void OnQuitting()
        {
            inputThread?.Abort();
            winkThread?.Abort();
            inputThread = null;
            winkThread = null;
        }

        private void OnDestroy()
        {
            if (KeyViewer)
                Destroy(KeyViewer.gameObject);

            BundleManager?.Dispose();
            harmony?.UnpatchAll(YoonKeyViewerInfo.PLUGIN_GUID);
        }
    }
}
#endif