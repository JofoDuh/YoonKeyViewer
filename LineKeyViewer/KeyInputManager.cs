using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

namespace LineKeyViewer;

public static class KeyInputManager {
    public static readonly int[] Location = [0, 1, 2, 3, 4, 5, 6, 7, 12, 13, 9, 8, 10, 11, 14, 15];
    [DllImport("user32.dll")]
    public static extern short GetAsyncKeyState(int vKey);

    private static bool CheckKey(KeyCode keyCode) => (int) keyCode < 0x1000 ? Input.GetKey(keyCode) : GetAsyncKeyState((int) keyCode - 0x1000) != 0;
    
    public static void ListenKey() {
        try {
            Setting setting = Main.Setting;
            Main main = Main.Instance;
            bool[] keyState = new bool[16];
            List<int> leftPressed = [];
            List<int> rightPressed = [];
            int mainCount = 0;
            while(main.Enabled) {
                KeyCode[] keyCodes = setting.KeyCodes;
                for(int i = 0; i < keyCodes.Length; i++) {
                    bool current = CheckKey(keyCodes[Location[i]]);
                    if(current == keyState[i]) continue;
                    bool left = i is >= 12 or >= 4 and < 8;
                    Key key = Main.Keys[i];
                    keyState[i] = current;
                    key.enable = (sbyte) (current ? 1 : 0);
                    List<int> pressed = left ? leftPressed : rightPressed;
                    if(current) {
                        pressed.Add(i);
                        (left ? Main.LeftHand : Main.RightHand).sprite = BundleManager.Instance.PressedKeySprites[i];
                    } else {
                        pressed.Remove(i);
                        (left ? Main.LeftHand : Main.RightHand).sprite = 
                            pressed.Count == 0 ? BundleManager.Instance.UnpressedKeySprites[left ? 0 : 1] : BundleManager.Instance.PressedKeySprites[pressed[^1]];
                    }
                    if(i >= 8) continue;
                    if(current) mainCount++;
                    else mainCount--;
                    if(mainCount < 8) {
                        if(!Main.HeadOn) continue;
                        Main.Head.enable = 0;
                        Main.LeftHand.enable = 1;
                        Main.RightHand.enable = 1;
                        if(Main.Setting.HideDesk) {
                            if(Main.WinkOn) {
                                Main.MainImage.sprite = Main.Setting.HideDesk ? BundleManager.Instance.Line : BundleManager.Instance.LineTable;
                                Main.WinkOn = false;
                            }
                            Main.MainImage.enable = 1;
                        } else {
                            Main.MainImage.sprite = Main.Setting.HideDesk ? BundleManager.Instance.Line : BundleManager.Instance.LineTable;
                            Main.WinkOn = false;
                        }
                        Main.HeadOn = false;
                    } else if(!Main.HeadOn) {
                        Main.Head.enable = 1;
                        Main.LeftHand.enable = 0;
                        Main.RightHand.enable = 0;
                        if(Main.Setting.HideDesk) Main.MainImage.enable = 0;
                        else Main.MainImage.sprite = BundleManager.Instance.Table;
                        Main.HeadOn = true;
                    }
                }
            }
        } catch (ThreadAbortException) {
        } catch (Exception e) {
            Main main = Main.Instance;
            if(!main.Enabled) return;
            main.LogReportException("Failed to listen key", e);
            main.Disable();
            main.ModEntry.Enabled = true;
            main.ModEntry.Info.DisplayName += " <color=red>[Error]</color>";
        }
    }
}