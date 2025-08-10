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
    class PatchStage4Boss
    {
        public static MethodBase TargetMethod()
        {
            return (MethodBase)(AccessTools.TypeByName("Shooting.Boss_Kage01").GetMember(".ctor", AccessTools.all)[0]);
        }
        // { "normal 1", "card 1", "normal 2", "card 2", "card 3", "card 4", "card 5",  "FSC" }
        public static void Postfix(BaseBoss __instance)
        {
            if (!PracSelection.is_Prac)
                return;
            if (PracSelection.comboBox_type_sel.SelectedIndex != (int)PracSelection.SelectedType.Boss)// not boss
                return;
            var lifes    = new int[] { 5, 5, 4, 4, 3, 2, 1, 1};
            var set_life = new int[] { 0, 1, 0, 1, 0, 0, 0, 0};
            var onspells = new int[] { 0, 1, 0, 1, 1, 1, 1, 1};
            StagePatch.BossJump(__instance, lifes, set_life, onspells);
        }
    }

    [HarmonyPatch]
    class PatchFSC4
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Shooting.EndBoss_FSC04:Ctrl");
        }
        public static void Postfix(BaseEffect __instance)
        {
            if (!PracSelection.is_Prac)
                return;
            if (PracSelection.comboBox_type_sel.SelectedIndex != (int)PracSelection.SelectedType.Boss)// not boss
                return;
            if (PracSelection.comboBox_subStage_sel.SelectedIndex != PracSelection.n_FSC[3])//not FSC
                return;
            StagePatch.BossJump_FSC(__instance);
        }
    }


    [HarmonyPatch]
    class PatchStage4Story
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Shooting.Planes.Story.Story_SSS04_01:Ctrl");
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
    class PatchStage4BeginBoss
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Shooting.GameState_SSS04:Drama");
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
                case PracSelection.SelectedType.Boss:
                    StagePatch.SetFSC_EnhanceCount(__instance, p_timeMain);
                    StagePatch.BeginBoss(__instance, p_timeMain, 8850, 8700, () => { __instance.StageData.ChangeBGM(".\\BGM\\Boss04.wav", 0, 0, 255, 463491, 7541982); });
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
