using ADOFAI;
using HarmonyLib;
using JALib.Core.Patch;
using MonsterLove.StateMachine;
using System;
using System.Collections.Generic;
using YoonKeyViewer.Component;

namespace YoonKeyViewer
{
    public static class Patch
    {
        [HarmonyPatch(typeof(StateBehaviour), nameof(StateBehaviour.ChangeState), new Type[] { typeof(Enum) })]
        static class ChangeStatePatch
        {
            static void Postfix(Enum newState)
            {
                switch ((States)newState)
                {
                    case States.Fail:
                    case States.Fail2:
                        KeyInputManager.Die(Characters.Yoon);
                        break;
                    case States.Won:
                        if (scrController.instance.noFail && scrController.instance.mistakesManager.GetDeaths() != 0) 
                            KeyInputManager.Die(Characters.Yoon);
                        else KeyInputManager.Clear(Characters.Yoon);
                        break;
                    default:
                        KeyInputManager.ResetPatch(Characters.Yoon);
                        break;
                }
            }
        }

        [HarmonyPatch(typeof(scrUIController), "WipeToBlack")]
        public static class WipeToBlackPatch
        {
            public static void Postfix()
            {
                Main.KeyViewer.isNervous = false;
                KeyInputManager.ResetPatch(Characters.Yoon);
            }
        }
        [HarmonyPatch(typeof(scnEditor), "ResetScene")]
        public static class ResetScenePatch
        {
            public static void Postfix()
            {
                Main.KeyViewer.isNervous = false;
                KeyInputManager.ResetPatch(Characters.Yoon);
            }
        }
        [HarmonyPatch(typeof(scnEditor), "SwitchToEditMode")]
        public static class SwitchToEditModePatch
        {
            public static void Postfix()
            {
                Main.KeyViewer.isNervous = false;
                KeyInputManager.ResetPatch(Characters.Yoon);
            }
        }

        [HarmonyPatch(typeof(scrPlanet), "MoveToNextFloor")]
        public static class MoveToNextFloorPatch
        {
            static float bpm = 0f;
            static float pitch = 1f;
            public static void Postfix(scrPlanet __instance)
            {
                double speed = __instance.controller.speed;
                pitch = ADOBase.conductor.song.pitch;
                bpm = ADOBase.conductor.bpm * pitch;
                //double floorBPM = getRealBPM(floor, bpm) * pitch;
                if ((bpm * speed) >= Main.setting.NervousBPM)
                {
                    Main.KeyViewer.isNervous = true;
                }
                else
                {
                    Main.KeyViewer.isNervous = false;
                }
                scrYoonKeyViewer keyViewer = Main.KeyViewer;
                keyViewer.Yoon.sprite = keyViewer.Yoon.image.sprite = keyViewer.isNervous ? YoonBundleManager.Instance.YoonNervous
                    : YoonBundleManager.Instance.YoonIdle;
            }
            public static double getRealBPM(scrFloor floor, float bpm)
            {
                if (floor == null)
                    return bpm;
                if (floor.nextfloor == null)
                    return ADOBase.controller.speed * bpm;
                return 60.0 / (floor.nextfloor.entryTime - floor.entryTime);
            }
        }
        [HarmonyPatch(typeof(scrController), "StartLoadingScene")]
        public static class StartLoadingScenePatch
        {
            public static void Postfix()
            {
                Main.KeyViewer.isNervous = false;
                KeyInputManager.ResetPatch(Characters.Yoon);
            }
        }
    }
}