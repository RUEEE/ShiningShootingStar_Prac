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
        //{"normal 1","card 1","normal 2","card 2","normal 3","card 3","normal 4","card 4","card 5","card 6"
        public static void Postfix(BaseBoss __instance)
        {
            if (!PracSelection.is_Prac)
                return;
            if (PracSelection.comboBox_type_sel.SelectedIndex != 1)// not boss
                return;
            var lifes = new int[] { 6, 6, 5, 5, 4, 4, 3, 3, 2, 1 ,1,1,1 };
            var set_life = new bool[] { false, true, false, true, false, true, false, true, false, false, false, false, false };
            var onspells = new bool[] { false, true, false, true, false, true, false, true, true, true, true, true, true };
            int id = PracSelection.comboBox_subStage_sel.SelectedIndex;
            if (id <= lifes.Length && id >= 0)
            {
                if(id >= 10)//LSC
                {
                    __instance.Life = lifes[id];
                    __instance.OnSpell = true;
                    __instance.Time = 102;
                    __instance.HealthPoint = 0;
                }
                else
                {
                    __instance.Life = lifes[id];
                    __instance.OnSpell = onspells[id];
                    if (set_life[id]) {
                        __instance.HealthPoint = __instance.SpellcardHP;
                        // __instance.Time = 0;
                    }
                }
                
            }
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
            if (PracSelection.comboBox_type_sel.SelectedIndex != 1)// not boss
                return;
            var lifes = new int[] { 6, 6, 5, 5, 4, 4, 3, 3, 2, 1,  3, 2, 1 };
            int id = PracSelection.comboBox_subStage_sel.SelectedIndex;
            if (id <= lifes.Length && id >= 10)//LSC
            {
                __instance.Life = lifes[id];
            }
        }
    }

    [HarmonyPatch]
    class PatchLSC
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Shooting.EndBoss_FSC06:Ctrl");
        }
        public static void Postfix(BaseEffect __instance)
        {
            if (!PracSelection.is_Prac)
                return;
            if (PracSelection.comboBox_type_sel.SelectedIndex != 1)// not boss
                return;
            if (PracSelection.comboBox_subStage_sel.SelectedIndex < 10)//not LSC
                return;
            if(__instance.Time < __instance.LifeTime - 20)
                __instance.Time = __instance.LifeTime - 20;
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
            if (PracSelection.comboBox_type_sel.SelectedIndex != 1)// not boss
                return;

            var type = AccessTools.GetDeclaredFields(AccessTools.TypeByName("Shooting.Planes.Story.BaseStory_SSS"));
            FieldInfo f_conv = null;
            foreach (var field in type)
            {
                if (field.Name=="Conv")
                {
                    f_conv = field;
                    break;
                }
            }
            if (f_conv==null)
                return;
            IList lst = (IList)f_conv.GetValue(__instance);
            __instance.Time = lst.Count;
            __instance.StageData.ChangeBGM(".\\BGM\\Boss06.wav", 0, 0, 255, 1697409, 10789065);
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
            if (PracSelection.comboBox_type_sel.SelectedIndex != 1)// not boss
                return;

            if (__instance == null)
                return;
            var type = AccessTools.GetDeclaredProperties(AccessTools.TypeByName("Shooting.BaseGameState"));
            PropertyInfo f_timeMain = null;
            foreach (var prop in type)
            {
                if (prop.Name == "TimeMain")
                {
                    f_timeMain = prop;
                    break;
                }
            }
            if (f_timeMain == null)
                return;
            //8400
            int time = (int)(f_timeMain.GetValue(__instance,null));
            if (time==8550-130)
            {
                f_timeMain.SetValue(__instance, 8549, null);
            }
        }
    }

}
