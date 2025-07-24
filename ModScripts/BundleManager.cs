using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace YoonKeyViewer
{
    public class LineBundleManager
    {
        public static LineBundleManager Instance;
        public readonly AssetBundle Bundle;
        public Sprite[] PressedKeySprites;
        public Sprite[] UnpressedKeySprites;
        public Sprite Line;
        public Sprite LineHead;
        public Sprite LineTable;
        public Sprite LineWink;
        public Sprite LineWinkTable;
        public Sprite Table;
        public Sprite LineClear;
        public Sprite LineClearTable;
        public Sprite LineDie;
        public GameObject KeyViewerObject;

        public LineBundleManager()
        {
            Bundle = AssetBundle.LoadFromFile(Path.Combine(Main.ModPath, "ykv_assets.bundle"));
            PressedKeySprites = new Sprite[16];
            UnpressedKeySprites = new Sprite[2];
            foreach (Object asset in Bundle.LoadAllAssets())
            {
                if (asset is GameObject gameObject)
                {
                    KeyViewerObject = gameObject;
                    continue;
                }
                if (asset is not Sprite sprite) continue;
                else if (asset.name.StartsWith("pressed_key")) PressedKeySprites[int.Parse(asset.name.Substring(11)) - 1] = sprite;
                else switch (asset.name)
                    {
                        case "Line1":
                            Line = sprite;
                            break;
                        case "Line_head":
                            LineHead = sprite;
                            break;
                        case "Line_table":
                            LineTable = sprite;
                            break;
                        case "Line_wink":
                            LineWink = sprite;
                            break;
                        case "Line_table_wink":
                            LineWinkTable = sprite;
                            break;
                        case "table":
                            Table = sprite;
                            break;
                        case "unpressed_left":
                            UnpressedKeySprites[0] = sprite;
                            break;
                        case "unpressed_right":
                            UnpressedKeySprites[1] = sprite;
                            break;
                        case "clear":
                            LineClear = sprite;
                            break;
                        case "clear_table":
                            LineClearTable = sprite;
                            break;
                        case "die":
                            LineDie = sprite;
                            break;
                    }
            }
        }

        public void Dispose()
        {
            Bundle.Unload(true);
            Instance = null;
        }
    }
    public class YoonBundleManager
    {
        public static YoonBundleManager Instance;
        public readonly AssetBundle Bundle;
        public Sprite[] PressedKeySprites;
        public Sprite[] UnpressedKeySprites;
        public Sprite[] FPressedKeySprites;
        public Sprite[] FUnpressedKeySprites;
        public Sprite YoonIdle;
        public Sprite YoonIdleWink;
        public Sprite Table;
        public Sprite YoonDie;
        public Sprite YoonClear;
        public Sprite YoonNervous;
        public Sprite YoonSmash;
        public Sprite YoonClearNoLeg;
        public GameObject KeyViewerObject;

        public YoonBundleManager()
        {
            Bundle = AssetBundle.LoadFromFile(Path.Combine(Main.ModPath, "ykv_assets.bundle"));
            PressedKeySprites = new Sprite[16];
            UnpressedKeySprites = new Sprite[2];
            FPressedKeySprites = new Sprite[2];
            FUnpressedKeySprites = new Sprite[2];
            foreach (Object asset in Bundle.LoadAllAssets())
            {
                if (asset is GameObject gameObject && ((GameObject)asset).name == "YoonKeyViewer")
                {
                    KeyViewerObject = gameObject;
                    continue;
                }
                if (asset is not Sprite sprite) continue;
                else if (asset.name.StartsWith("pressed_key")) PressedKeySprites[int.Parse(asset.name.Substring(11)) - 1] = sprite;
                else switch (asset.name)
                    {
                        case "YOON1":
                            YoonIdle = sprite;
                            break;
                        case "wink1":
                            YoonIdleWink = sprite;
                            break;
                        case "YOON_press_all":
                            YoonSmash = sprite;
                            break;
                        case "desk1":
                            Table = sprite;
                            break;
                        case "unpressed_left":
                            UnpressedKeySprites[0] = sprite;
                            break;
                        case "unpressed_right":
                            UnpressedKeySprites[1] = sprite;
                            break;
                        case "clear_motion":
                            YoonClear = sprite;
                            break;
                        case "clear_motion_noleg":
                            YoonClearNoLeg = sprite;
                            break;
                        case "face_highBPM":
                            YoonNervous = sprite;
                            break;
                        case "face_die":
                            YoonDie = sprite;
                            break;
                        case "leg_left":
                            FUnpressedKeySprites[0] = sprite;
                            break;
                        case "leg_right":
                            FUnpressedKeySprites[1] = sprite;
                            break;
                        case "pressed_leg_left":
                            FPressedKeySprites[0] = sprite;
                            break;
                        case "pressed_leg_right":
                            FPressedKeySprites[1] = sprite;
                            break;
                    }
            }
        }

        public void Dispose()
        {
            Bundle.Unload(true);
            Instance = null;
        }
    }
}