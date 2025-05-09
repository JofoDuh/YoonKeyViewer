using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LineKeyViewer;

public class BundleManager {
    public static BundleManager Instance;
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

    public BundleManager() {
        Bundle = AssetBundle.LoadFromFile(Path.Combine(Main.Instance.Path, "linekeyviewer"));
        PressedKeySprites = new Sprite[16];
        UnpressedKeySprites = new Sprite[2];
        foreach(Object asset in Bundle.LoadAllAssets()) {
            if(asset is GameObject gameObject) {
                KeyViewerObject = gameObject;
                continue;
            }
            if(asset is not Sprite sprite) continue;
            else if(asset.name.StartsWith("pressed_key")) PressedKeySprites[int.Parse(asset.name[11..]) - 1] = sprite;
            else switch(asset.name) {
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

    public void Dispose() {
        Bundle.Unload(true);
        Instance = null;
    }
}