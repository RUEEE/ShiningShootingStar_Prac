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
    class PatchStage7Boss
    {
        public static MethodBase TargetMethod()
        {
            return (MethodBase)(AccessTools.TypeByName("Shooting.Boss_Rika01").GetMember(".ctor", AccessTools.all)[0]);
        }
        // { "normal 1", "card 1", "normal 2", "card 2", "normal 3", "card 3", "normal 4", "card 4",
        // "normal 5", "card 5", "normal 6", "card 6", "normal 7", "card 7", "normal 8", "card 8", "card 9", "card 10", "FSC" }
        public static void Postfix(BaseBoss __instance)
        {
            if (!PracSelection.is_Prac)
                return;
            if (PracSelection.comboBox_type_sel.SelectedIndex != (int)PracSelection.SelectedType.Boss)// not boss
                return;

            var lifes   = new int[]  {10, 10, 9, 9, 8, 8, 7, 7, 6, 6, 5, 5, 4, 4, 3, 3, 2, 1, 1 };
            var set_life = new int[] { 0,  1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0, 0 };
            var onspells = new int[] { 0,  1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 1, 1, 1 };
            StagePatch.BossJump(__instance, lifes, set_life, onspells);
        }
    }

    [HarmonyPatch]
    class PatchStage7MidBoss
    {
        public static MethodBase TargetMethod()
        {
            return (MethodBase)(AccessTools.TypeByName("Shooting.Boss_Seiryuu03").GetMember(".ctor", AccessTools.all)[0]);
        }
        //  { "card 1", "card 2" , "card 3" }
        public static void Postfix(BaseBoss __instance)
        {
            if (!PracSelection.is_Prac)
                return;
            if (PracSelection.comboBox_type_sel.SelectedIndex != (int)PracSelection.SelectedType.MidBoss)
                return;

            var lifes = new int[] { 3, 2, 1 };
            var set_life = new int[] { 0, 0, 0 };
            var onspells = new int[] { 1, 1, 1 };
            __instance.OriginalPosition = __instance.DestPoint;
            StagePatch.BossJump(__instance, lifes, set_life, onspells, StagePatch.NO_FSC);
        }
    }


    [HarmonyPatch]
    class PatchFSC7
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Shooting.EndBoss_FSCEx:Ctrl");
        }
        public static void Postfix(BaseEffect __instance)
        {
            if (!PracSelection.is_Prac)
                return;
            if (PracSelection.comboBox_type_sel.SelectedIndex != (int)PracSelection.SelectedType.Boss)// not boss
                return;
            if (PracSelection.comboBox_subStage_sel.SelectedIndex != PracSelection.n_FSC[6])//not FSC
                return;
            StagePatch.BossJump_FSC(__instance);
        }
    }


    [HarmonyPatch]
    class PatchStage7Story
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Shooting.Planes.Story.Story_SSSEx_01:Ctrl");
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
    class PatchStage7BeginBoss
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Shooting.GameState_SSSEx:Drama");
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
                        StagePatch.BeginMidBoss(__instance, p_timeMain, 4920);
                    }
                    break;
                case PracSelection.SelectedType.Boss:
                    {
                        StagePatch.SetFSC_EnhanceCount(__instance, p_timeMain);
                        int time = (int)(p_timeMain.GetValue(__instance, null));
                        if (time == 2)
                        {
                            p_timeMain.SetValue(__instance, 10260 - 130 - 1, null);
                        }else if (time == 10260 - 130)
                        {
                            p_timeMain.SetValue(__instance, 10260 - 1, null);
                        }
                        else if (time == 10260)//testStartTime
                        {
                            __instance.StageData.ChangeBGM(".\\BGM\\BossEx.wav", 0, 0, 255, 1125873, 10783332);
                        }
                    }
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
