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
    class PatchStage2Boss
    {
        public static MethodBase TargetMethod()
        {
            return (MethodBase)(AccessTools.TypeByName("Shooting.Boss_Rakuki02").GetMember(".ctor", AccessTools.all)[0]);
        }
        // { "normal 1", "card 1", "normal 2", "card 2", "card 3", "FSC" }
        public static void Postfix(BaseBoss __instance)
        {
            if (!PracSelection.is_Prac)
                return;
            if (PracSelection.comboBox_type_sel.SelectedIndex != (int)PracSelection.SelectedType.Boss)// not boss
                return;
            var lifes    = new int[] { 3, 3, 2, 2, 1, 1 };
            var set_life = new int[] { 0, 1, 0, 1, 0, 0 };
            var onspells = new int[] { 0, 1, 0, 1, 1, 1 };
            StagePatch.BossJump(__instance, lifes, set_life, onspells);
        }
    }

    [HarmonyPatch]
    class PatchStage2MidBoss
    {
        public static MethodBase TargetMethod()
        {
            return (MethodBase)(AccessTools.TypeByName("Shooting.Boss_Rakuki01").GetMember(".ctor", AccessTools.all)[0]);
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
    class PatchFSC2
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Shooting.EndBoss_FSC02:Ctrl");
        }
        public static void Postfix(BaseEffect __instance)
        {
            if (!PracSelection.is_Prac)
                return;
            if (PracSelection.comboBox_type_sel.SelectedIndex != (int)PracSelection.SelectedType.Boss)// not boss
                return;
            if (PracSelection.comboBox_subStage_sel.SelectedIndex !=  PracSelection.n_FSC[1])//not FSC
                return;
            StagePatch.BossJump_FSC(__instance);
        }
    }



    [HarmonyPatch]
    class PatchStage2StoryA
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Shooting.Planes.Story.Story_SSS02_01A:Ctrl");
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
    class PatchStage2StoryB
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Shooting.Planes.Story.Story_SSS02_01B:Ctrl");
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
    class PatchStage2StoryC
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Shooting.Planes.Story.Story_SSS02_01C:Ctrl");
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
    class PatchStage2StoryD
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Shooting.Planes.Story.Story_SSS02_01D:Ctrl");
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
    class PatchStage2BeginBoss
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Shooting.GameState_SSS02:Drama");
        }

        public static void Postfix(IGameState __instance)
        {
            StagePatch.RecordTimes(__instance);

            if (!PracSelection.is_Prac)
                return;

            var p_timeMain = StagePatch.GetTimeMain(__instance);
            if (p_timeMain == null)
                return;
            StagePatch.RecordTimes(__instance,p_timeMain);

            PracSelection.SelectedType type = (PracSelection.SelectedType)PracSelection.comboBox_type_sel.SelectedIndex;
            switch (type)
            {
                case PracSelection.SelectedType.MidBoss:
                    StagePatch.BeginMidBoss(__instance, p_timeMain, 4680);
                    break;
                case PracSelection.SelectedType.Boss:
                    StagePatch.SetFSC_EnhanceCount(__instance, p_timeMain);
                    StagePatch.BeginBoss(__instance, p_timeMain, 7890, 7740, () => { __instance.StageData.ChangeBGM(".\\BGM\\Boss02.wav", 0, 0, 255, 327663, 3504627); });
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
