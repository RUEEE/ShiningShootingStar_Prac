using HarmonyLib;
using Shooting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace SSS_Prac_Launcher
{
    [HarmonyPatch]
    class PatchStage5Boss
    {
        public static MethodBase TargetMethod()
        {
            return (MethodBase)(AccessTools.TypeByName("Shooting.Boss_Rakukun01").GetMember(".ctor", AccessTools.all)[0]);
        }
        // { "normal 1", "card 1", "normal 2", "card 2", "normal 3", "card 3", "card 4", "card 5", "FSC" }
        public static void Postfix(BaseBoss __instance)
        {
            if (!PracSelection.is_Prac)
                return;
            if (PracSelection.comboBox_type_sel.SelectedIndex != (int)PracSelection.SelectedType.Boss)// not boss
                return;
            var lifes    = new int[] { 5, 5, 4, 4, 3, 3, 2, 1, 1};
            var set_life = new int[] { 0, 1, 0, 1, 0, 1, 0, 0, 0};
            var onspells = new int[] { 0, 1, 0, 1, 0, 1, 1, 1, 1};
            StagePatch.BossJump(__instance, lifes, set_life, onspells);
        }
    }

    [HarmonyPatch]
    class PatchStage5MidBoss
    {
        public static MethodBase TargetMethod()
        {
            return (MethodBase)(AccessTools.TypeByName("Shooting.Boss_Rakuki04").GetMember(".ctor", AccessTools.all)[0]);
        }
        // { "normal 1", "card 1"}
        public static void Postfix(BaseBoss __instance)
        {
            if (!PracSelection.is_Prac)
                return;
            if (PracSelection.comboBox_type_sel.SelectedIndex != (int)PracSelection.SelectedType.MidBoss)
                return;
            var lifes = new int[] { 1, 1 };
            var set_life = new int[] { 0, 1 };
            var onspells = new int[] { 0, 1 };
            __instance.OriginalPosition = __instance.DestPoint;
            StagePatch.BossJump(__instance, lifes, set_life, onspells, StagePatch.NO_FSC);
        }
    }


    [HarmonyPatch]
    class PatchFSC5
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Shooting.EndBoss_FSC05:Ctrl");
        }
        public static void Postfix(BaseEffect __instance)
        {
            if (!PracSelection.is_Prac)
                return;
            if (PracSelection.comboBox_type_sel.SelectedIndex != (int)PracSelection.SelectedType.Boss)// not boss
                return;
            if (PracSelection.comboBox_subStage_sel.SelectedIndex != PracSelection.n_FSC[4])//not FSC
                return;
            StagePatch.BossJump_FSC(__instance);
        }
    }


    [HarmonyPatch]
    class PatchStage5Story
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Shooting.Planes.Story.Story_SSS05_02:Ctrl");
        }
        public static void Prefix(BaseStory __instance)
        {
            if (!PracSelection.is_Prac)
                return;
            if (PracSelection.comboBox_type_sel.SelectedIndex != (int)PracSelection.SelectedType.Boss)// not boss
                return;
            StagePatch.JumpStory(__instance);
        }
    }

    [HarmonyPatch]
    class PatchStage5MidStory
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Shooting.Planes.Story.Story_SSS05_01:Ctrl");
        }
        public static void Prefix(BaseStory __instance)
        {
            if (!PracSelection.is_Prac)
                return;
            if (PracSelection.comboBox_type_sel.SelectedIndex != (int)PracSelection.SelectedType.MidBoss)// not boss
                return;
            StagePatch.JumpStory(__instance);
        }
    }

    [HarmonyPatch]
    class PatchStage5BeginBoss
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Shooting.GameState_SSS05:Drama");
        }

        public static void Postfix(IGameState __instance)
        {
            StagePatch.RecordTimes(__instance);

            if (!PracSelection.is_Prac)
                return;

            var p_timeMain = StagePatch.GetTimeMain(__instance);
            if (p_timeMain == null)
                return;
            StagePatch.RecordTimes(__instance, p_timeMain);

            PracSelection.SelectedType type = (PracSelection.SelectedType)PracSelection.comboBox_type_sel.SelectedIndex;
            switch (type)
            {
                case PracSelection.SelectedType.MidBoss:
                    {
                        StagePatch.BeginMidBoss(__instance, p_timeMain, 2500);
                        //skip stage 5 midboss story
                        int time = (int)(p_timeMain.GetValue(__instance, null));
                        if (time == 2501)
                        {
                            p_timeMain.SetValue(__instance, 2500 + 90 - 1, null);
                        }
                    }
                    break;
                case PracSelection.SelectedType.Boss:
                    StagePatch.SetFSC_EnhanceCount(__instance, p_timeMain);
                    StagePatch.BeginBoss(__instance, p_timeMain, 8750, 8600, () => { __instance.StageData.ChangeBGM(".\\BGM\\Boss05.wav", 0, 0, 255, 1000056, 6599345); });
                    break;
                case PracSelection.SelectedType.Road:
                    StagePatch.RoadJump(__instance, p_timeMain);
                    break;
                default:
                    break;
            }
        }
    }

}
