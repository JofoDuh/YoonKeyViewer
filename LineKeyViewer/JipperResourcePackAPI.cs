using System;
using JALib.Tools;
using JipperResourcePack.Keyviewer;
using UnityEngine;

namespace LineKeyViewer;

public class JipperResourcePackAPI {
    private static bool Initialized;

    public static bool CheckJipperResourcePack() {
        if(Initialized) return true;
        try {
            Check();
            Initialized = true;
            if(Main.Setting.ShareJipperResourcePack) Main.Setting.ShareJipperKeyCode(true);
        } catch (Exception) {
            // ignored
        }
        return Initialized;
    }

    private static bool Check() => JipperResourcePack.Main.Instance.Enabled;

    public static KeyCode[] GetKey16() => KeyViewer.Settings.key16;

    public static void UpdateKeyLimit() => typeof(KeyViewer).Invoke("UpdateKeyLimit");

    public static void SaveSetting() => JipperResourcePack.Main.Instance.SaveSetting();
}