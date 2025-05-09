using System;
using JALib.Core.Patch;
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
        if(Main.GameResult) return;
        Main.GameResult = true;
        bool head = Main.HeadOn;
        Main.HeadOn = false;
        Main.WinkOn = false;
        Main.Head.sprite = Main.Head.image.sprite = BundleManager.Instance.LineDie;
        if(head) return;
        if(Main.Setting.HideDesk) Main.MainImage.enable = 0;
        else Main.MainImage.sprite = BundleManager.Instance.Table;
        Main.LeftHand.enable = 0;
        Main.RightHand.enable = 0;
        Main.Head.enable = 1;
    }

    public static void Clear() {
        if(Main.GameResult) return;
        Main.GameResult = true;
        bool head = Main.HeadOn;
        Main.HeadOn = false;
        Main.WinkOn = false;
        Main.MainImage.sprite = Main.MainImage.image.sprite = Main.Setting.HideDesk ? BundleManager.Instance.LineClear : BundleManager.Instance.LineClearTable;
        if(head) {
            Main.MainImage.enable = 1;
            Main.Head.enable = 0;
        } else {
            Main.LeftHand.enable = 0;
            Main.RightHand.enable = 0;
        }
    }
    
    [JAPatch(typeof(scrUIController), "WipeToBlack", PatchType.Postfix, false)]
    [JAPatch(typeof(scnEditor), "ResetScene", PatchType.Postfix, false)]
    [JAPatch(typeof(scrController), "StartLoadingScene", PatchType.Postfix, false)]
    public static void Reset() {
        Main.GameResult = false;
        Main.MainImage.sprite = Main.Setting.HideDesk ? BundleManager.Instance.Line : BundleManager.Instance.LineTable;
        Main.LeftHand.enable = 1;
        Main.RightHand.enable = 1;
        if(Main.Head.image.enabled) {
            Main.Head.enable = 0;
            Main.Head.image.sprite = BundleManager.Instance.LineHead;
            if(Main.Setting.HideDesk) Main.MainImage.enable = 1;
        }
        Main.GameResult = false;
    }
}