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
    class PatchStage6Boss
    {
        public static MethodBase TargetMethod()
        {
            return (MethodBase)(AccessTools.TypeByName("Shooting.Boss_Tensei01").GetMember(".ctor", AccessTools.all)[0]);
        }
        //{"normal 1","card 1","normal 2","card 2","normal 3","card 3","normal 4","card 4","card 5","card 6"}
        public static void Postfix(BaseBoss __instance)
        {
            if (!PracSelection.is_Prac)
                return;
            if (PracSelection.comboBox_type_sel.SelectedIndex != (int)PracSelection.SelectedType.Boss)// not boss
                return;
            var lifes    = new int[] { 6, 6, 5, 5, 4, 4, 3, 3, 2, 1, 1, 1, 1 };
            var set_life = new int[] { 0, 1, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0, 0 };
            var onspells = new int[] { 0, 1, 0, 1, 0, 1, 0, 1, 1, 1, 1, 1, 1 };
            int id = PracSelection.comboBox_subStage_sel.SelectedIndex;
            StagePatch.BossJump(__instance, lifes, set_life, onspells,10);
        }
    }

    [HarmonyPatch]
    class PatchStage6Boss2
    {
        public static MethodBase TargetMethod()
        {
            return (MethodBase)(AccessTools.TypeByName("Shooting.Boss_Tensei02").GetMember(".ctor", AccessTools.all)[0]);
        }
        //{"normal 1","card 1","normal 2","card 2","normal 3","card 3","normal 4","card 4","card 5","card 6"
        public static void Postfix(BaseBoss __instance)
        {
            if (!PracSelection.is_Prac)
                return;
            if (PracSelection.comboBox_type_sel.SelectedIndex != (int)PracSelection.SelectedType.Boss)// not boss
                return;
            var lifes = new int[] { 6, 6, 5, 5, 4, 4, 3, 3, 2, 1,  3, 2, 1 };
            int id = PracSelection.comboBox_subStage_sel.SelectedIndex;
            if (id <= lifes.Length && id >= 10)//FSC
            {
                __instance.Life = lifes[id];
            }
        }
    }

    [HarmonyPatch]
    class PatchFSC6
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Shooting.EndBoss_FSC06:Ctrl");
        }
        public static void Postfix(BaseEffect __instance)
        {
            if (!PracSelection.is_Prac)
                return;
            if (PracSelection.comboBox_type_sel.SelectedIndex != (int)PracSelection.SelectedType.Boss)// not boss
                return;
            if (PracSelection.comboBox_subStage_sel.SelectedIndex < 10)//not FSC
                return;
            StagePatch.BossJump_FSC(__instance);
        }
    }


    [HarmonyPatch]
    class PatchStage6Story
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Shooting.Planes.Story.Story_SSS06_01:Ctrl");
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
    class PatchStage6BeginBoss
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Shooting.GameState_SSS06:Drama");
        }

        public static void Postfix(IGameState __instance)
        {
            if (!PracSelection.is_Prac)
                return;
            if (PracSelection.comboBox_type_sel.SelectedIndex != (int)PracSelection.SelectedType.Boss)// not boss
                return;
            //8400
            StagePatch.BeginBoss(__instance, 8550, 8400, () => { __instance.StageData.ChangeBGM(".\\BGM\\Boss06.wav", 0, 0, 255, 1697409, 10789065); });
        }
    }

}
