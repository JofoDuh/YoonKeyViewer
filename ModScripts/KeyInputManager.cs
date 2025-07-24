using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using YoonKeyViewer.Component;
using UnityEngine;

namespace YoonKeyViewer
{

    public static class KeyInputManager
    {
        public static readonly int[] HandLocation = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 12, 13, 9, 8, 10, 11, 14, 15 };        
        public static readonly int[] LegLocation = new int[] { 0, 1, 2, 3 };
        public static bool Reset;
        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);
        private static bool CheckKey(KeyCode keyCode) => (int)keyCode < 0x1000 ? Input.GetKey(keyCode) : GetAsyncKeyState((int)keyCode - 0x1000) != 0;

        public static void ListenKey()
        {
            try
            {
                Setting setting = Main.setting;
                bool[] keyState = new bool[16];
                List<int> leftPressed = new();
                List<int> rightPressed = new();
                int mainCount = 0;
                scrYoonKeyViewer keyViewer = Main.KeyViewer;
                while (Main.IsEnabled)
                {
                    if (Reset)
                    {
                        foreach (Key key in keyViewer.keys) key.enable = 0;
                        foreach (Key fkey in keyViewer.fKeys) fkey.enable = 0;
                        keyState = new bool[16];
                        leftPressed.Clear();
                        rightPressed.Clear();
                        keyViewer.leftHand.sprite = YoonBundleManager.Instance.UnpressedKeySprites[0];
                        keyViewer.rightHand.sprite = YoonBundleManager.Instance.UnpressedKeySprites[1];
                        keyViewer.leftLeg.sprite = YoonBundleManager.Instance.FUnpressedKeySprites[0];
                        keyViewer.rightLeg.sprite = YoonBundleManager.Instance.FUnpressedKeySprites[1];
                        mainCount = 0;
                        Reset = false;
                    }
                    KeyCode[] keyCodes = setting.KeyCodes;
                    KeyCode[] FkeyCodes = setting.FKeyCodes;
                    for (int i = 0; i < keyCodes.Length; i++)
                    {
                        bool current = CheckKey(keyCodes[HandLocation[i]]);
                        if (current == keyState[i]) continue;
                        int num = setting.FlipHorizontal ? (i < 8 ? 7 : 23) - i : i;
                        bool left = num < 4 || (num >= 8 && num < 12);
                        Key key = keyViewer.keys[num];
                        keyState[i] = current;
                        key.enable = (sbyte)(current ? 1 : 0);
                        List<int> pressed = left ? leftPressed : rightPressed;
                        if (current)
                        {
                            pressed.Add(num);
                            (left ? keyViewer.leftHand : keyViewer.rightHand).sprite = YoonBundleManager.Instance.PressedKeySprites[num];
                        }
                        else
                        {
                            pressed.Remove(num);
                            (left ? keyViewer.leftHand : keyViewer.rightHand).sprite =
                                pressed.Count == 0 ? YoonBundleManager.Instance.UnpressedKeySprites[left ? 0 : 1] : YoonBundleManager.Instance.PressedKeySprites[pressed[pressed.Count - 1]];
                        }
                        if (i >= 8) continue;
                        if (current) mainCount++;
                        else mainCount--;
                        if (keyViewer.gameResult) continue;
                        if (mainCount < 8)
                        {
                            if (!keyViewer.isSmashing) continue;
                            keyViewer.YoonSmash.sprite = keyViewer.YoonSmash.image.sprite = YoonBundleManager.Instance.YoonSmash;
                            keyViewer.Yoon.enable = 1;
                            keyViewer.leftHand.enable = 1;
                            keyViewer.rightHand.enable = 1;
                            keyViewer.YoonSmash.enable = 0;
                            if (Main.setting.HideDesk)
                            {
                                if (keyViewer.winkOn)
                                {
                                    keyViewer.Table.enable = Main.setting.HideDesk ? (sbyte)0 : (sbyte)1;
                                    keyViewer.winkOn = false;
                                }
                            }
                            else
                            {
                                keyViewer.winkOn = false;
                            }
                            keyViewer.isSmashing = false;
                        }
                        else if (!keyViewer.isSmashing)
                        {
                            keyViewer.Yoon.enable = 0;
                            keyViewer.leftHand.enable = 0;
                            keyViewer.rightHand.enable = 0;
                            keyViewer.YoonSmash.enable = 1;
                            keyViewer.isSmashing = true;
                        }
                    }
                    // Track foot key states
                    bool[] footStates = new bool[4];
                    if (!Main.setting.HideFeet)
                    {
                        if (!Main.setting.HideFeetKeyboard)
                        {
                            if (keyViewer.FeetKeyboard.enable == 0) keyViewer.FeetKeyboard.enable = 1;
                            for (int i = 0; i < FkeyCodes.Length; i++)
                            {
                                bool current = CheckKey(FkeyCodes[LegLocation[i]]);
                                footStates[i] = current;

                                Key fkey = keyViewer.fKeys[i];
                                fkey.enable = (sbyte)(current ? 1 : 0);
                            }

                            // LEFT LEG: F2 and F3
                            if (footStates[0] || footStates[1])
                            {
                                keyViewer.leftLeg.sprite = YoonBundleManager.Instance.FPressedKeySprites[0];
                            }
                            else
                            {
                                keyViewer.leftLeg.sprite = YoonBundleManager.Instance.FUnpressedKeySprites[0];
                            }

                            // RIGHT LEG: F7 and F8
                            if (footStates[2] || footStates[3])
                            {
                                keyViewer.rightLeg.sprite = YoonBundleManager.Instance.FPressedKeySprites[1];
                            }
                            else
                            {
                                keyViewer.rightLeg.sprite = YoonBundleManager.Instance.FUnpressedKeySprites[1];
                            }
                        }               
                    }
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception e)
            {
                Main.Logger.Log($"Issue with KeyInputManager{e.Message}");
            }
        }

        public static void Die(Characters characters)
        {
            switch (characters)
            {
                case (Characters.Line):
                    break;
                case (Characters.Yoon):
                    scrYoonKeyViewer keyViewer = Main.KeyViewer;
                    if (keyViewer.gameResult) return;
                    keyViewer.gameResult = true;
                    keyViewer.isSmashing = false;
                    keyViewer.winkOn = false;
                    keyViewer.YoonClear.sprite = keyViewer.YoonClear.image.sprite = YoonBundleManager.Instance.YoonDie;
                    keyViewer.YoonClear.enable = 1;
                    keyViewer.Yoon.enable = 0;
                    keyViewer.Table.enable = Main.setting.HideDesk ? (sbyte)0 : (sbyte)1;
                    keyViewer.leftHand.enable = 1;
                    keyViewer.rightHand.enable = 1;
                    keyViewer.leftLeg.enable = Main.setting.HideFeet ? (sbyte)0 : (sbyte)1;
                    keyViewer.rightLeg.enable = Main.setting.HideFeet ? (sbyte)0 : (sbyte)1;
                    keyViewer.FeetKeyboard.enable = Main.setting.HideFeet ? (sbyte)0 : Main.setting.HideFeetKeyboard ? (sbyte)0 : (sbyte)1;
                    keyViewer.YoonSmash.enable = 0;
                    break;
            }
        }
        public static void Clear(Characters characters)
        {
            switch (characters)
            {
                case (Characters.Line):
                    break;
                case (Characters.Yoon):
                    scrYoonKeyViewer keyViewer = Main.KeyViewer;
                    if (keyViewer.gameResult) return;
                    keyViewer.gameResult = true;
                    keyViewer.isSmashing = false;
                    keyViewer.winkOn = false;
                    keyViewer.Table.enable = Main.setting.HideDesk ? (sbyte)0 : (sbyte)1;
                    keyViewer.Yoon.enable = 0;
                    keyViewer.leftHand.enable = 0;
                    keyViewer.rightHand.enable = 0;
                    keyViewer.leftLeg.enable = 0;
                    keyViewer.rightLeg.enable = 0;
                    keyViewer.YoonSmash.enable = 0;
                    keyViewer.YoonClear.sprite = keyViewer.YoonClear.image.sprite
                        = Main.setting.HideFeet ? YoonBundleManager.Instance.YoonClearNoLeg : YoonBundleManager.Instance.YoonClear;
                    keyViewer.FeetKeyboard.enable = Main.setting.HideFeet ? (sbyte)0 : Main.setting.HideFeetKeyboard ? (sbyte)0 : (sbyte)1;
                    keyViewer.YoonClear.enable = 1;
                    break;
            }
        }
        public static void ResetPatch(Characters characters)
        {
            switch (characters)
            {
                case (Characters.Line):
                    break;
                case (Characters.Yoon):
                    scrYoonKeyViewer keyViewer = Main.KeyViewer;
                    keyViewer.Table.enable = Main.setting.HideDesk ? (sbyte)0 : (sbyte)1;
                    if (!keyViewer.gameResult)
                    {
                        keyViewer.Yoon.sprite = keyViewer.Yoon.image.sprite = keyViewer.isNervous ? YoonBundleManager.Instance.YoonNervous
                            : YoonBundleManager.Instance.YoonIdle;
                    }
                    keyViewer.Yoon.enable = 1;
                    keyViewer.leftHand.enable = 1;
                    keyViewer.rightHand.enable = 1;
                    keyViewer.leftLeg.enable = Main.setting.HideFeet ? (sbyte)0 : (sbyte)1;
                    keyViewer.rightLeg.enable = Main.setting.HideFeet ? (sbyte)0 : (sbyte)1;
                    keyViewer.FeetKeyboard.enable = Main.setting.HideFeet ? (sbyte)0 : Main.setting.HideFeetKeyboard ? (sbyte)0 : (sbyte)1;
                    if (keyViewer.YoonClear.image.enabled && keyViewer.gameResult)
                    {
                        keyViewer.Yoon.enable = 0;
                        keyViewer.YoonClear.enable = 1;
                    }
                    else
                    {
                        keyViewer.YoonClear.enable = 0;
                    }
                    if (keyViewer.YoonSmash.image.enabled && keyViewer.isSmashing)
                    {
                        keyViewer.Yoon.enable = 0;
                        keyViewer.leftHand.enable = 0;
                        keyViewer.rightHand.enable = 0;
                        keyViewer.YoonSmash.enable = 1;
                    }
                    else
                    {
                        keyViewer.YoonSmash.enable = 0;
                        keyViewer.YoonSmash.image.sprite = YoonBundleManager.Instance.YoonSmash;
                    }
                    keyViewer.gameResult = false;
                    break;
            }
        }
    }
}