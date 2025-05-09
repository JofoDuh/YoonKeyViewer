using System;
using JALib.Core.Patch;
using LineKeyViewer.Component;
using MonsterLove.StateMachine;

namespace LineKeyViewer;

public static class ResultHandler {
    [JAPatch(typeof(StateBehaviour), nameof(ChangeState), PatchType.Postfix, true, ArgumentTypesType = [typeof(Enum)])]
    public static void ChangeState(Enum newState) {
        switch((States) newState) {
            case States.Fail:
            case States.Fail2: 
                Die(); 
                break;
            case States.Won:
                if(scrController.instance.noFail && scrController.instance.mistakesManager.GetDeaths() != 0) Die();
                else Clear();
                break;
            default:
                Reset();
                break;
        }
    }

    public static void Die() {
        scrLineKeyViewer keyViewer = Main.KeyViewer;
        if(keyViewer.gameResult) return;
        keyViewer.gameResult = true;
        bool head = keyViewer.headOn;
        keyViewer.headOn = false;
        keyViewer.winkOn = false;
        keyViewer.head.sprite = keyViewer.head.image.sprite = BundleManager.Instance.LineDie;
        if(head) return;
        if(Main.Setting.HideDesk) keyViewer.mainImage.enable = 0;
        else keyViewer.mainImage.sprite = BundleManager.Instance.Table;
        keyViewer.leftHand.enable = 0;
        keyViewer.rightHand.enable = 0;
        keyViewer.head.enable = 1;
    }

    public static void Clear() {
        scrLineKeyViewer keyViewer = Main.KeyViewer;
        if(keyViewer.gameResult) return;
        keyViewer.gameResult = true;
        bool head = keyViewer.headOn;
        keyViewer.headOn = false;
        keyViewer.winkOn = false;
        keyViewer.mainImage.sprite = keyViewer.mainImage.image.sprite = Main.Setting.HideDesk ? BundleManager.Instance.LineClear : BundleManager.Instance.LineClearTable;
        if(head) {
            keyViewer.mainImage.enable = 1;
            keyViewer.head.enable = 0;
        } else {
            keyViewer.leftHand.enable = 0;
            keyViewer.rightHand.enable = 0;
        }
    }
    
    [JAPatch(typeof(scrUIController), "WipeToBlack", PatchType.Postfix, false)]
    [JAPatch(typeof(scnEditor), "ResetScene", PatchType.Postfix, false)]
    [JAPatch(typeof(scrController), "StartLoadingScene", PatchType.Postfix, false)]
    public static void Reset() {
        scrLineKeyViewer keyViewer = Main.KeyViewer;
        keyViewer.mainImage.sprite = Main.Setting.HideDesk ? BundleManager.Instance.Line : BundleManager.Instance.LineTable;
        keyViewer.leftHand.enable = 1;
        keyViewer.rightHand.enable = 1;
        if(keyViewer.head.image.enabled) {
            keyViewer.head.enable = 0;
            keyViewer.head.image.sprite = BundleManager.Instance.LineHead;
            if(Main.Setting.HideDesk) keyViewer.mainImage.enable = 1;
        }
        keyViewer.gameResult = false;
    }
}