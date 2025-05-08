using System;
using System.Reflection;
using JALib.Core;
using JALib.Core.Setting;
using JALib.Tools;
using JipperResourcePack.Keyviewer;
using UnityEngine;

namespace LineKeyViewer;

public class JipperResourcePackAPI {
    private static JipperResourcePackAPI Instance;
    private JAMod Mod;
    private JASetting Setting;
    private Action UpdateKeyLimitAction;

    private JipperResourcePackAPI(JAMod mod) {
        Mod = mod;
        Type keyViewerType = mod.GetType().Assembly.GetType("JipperResourcePack.Keyviewer.KeyViewer");
        Setting = keyViewerType.GetValue<JASetting>("Settings");
        UpdateKeyLimitAction = (Action) keyViewerType.Method("UpdateKeyLimit").CreateDelegate(typeof(Action));
    }

    public static JipperResourcePackAPI GetAPI() {
        if(Instance != null) return Instance;
        try {
            JAMod mod = JAMod.GetMods("JipperResourcePack");
            if(mod != null) Instance = new JipperResourcePackAPI(mod);
        } catch (Exception) {
            // ignored
        }
        return Instance;
    }

    public static bool CheckJipperResourcePack() => GetAPI() != null;

    public static KeyCode[] GetKey16() => Instance?.Setting?.GetValue<KeyCode[]>("key16");

    public static void UpdateKeyLimit() => Instance?.UpdateKeyLimitAction();

    public static void SaveSetting() => Instance?.Mod.SaveSetting();
}