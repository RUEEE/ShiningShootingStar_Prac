using HarmonyLib;
using Shooting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SSS_Prac_Launcher
{
    public class StagePatch
    {
        public static void BossJump(BaseBoss __instance,int[] lifes, int[] set_life, int[] onspells,int FSC=-1)
        {
            if (FSC==-1)
                FSC = lifes.Length - 1;
            int id = PracSelection.comboBox_subStage_sel.SelectedIndex;
            if (id <= lifes.Length && id >= 0)
            {
                if (id >= FSC)//FSC
                {
                    __instance.Life = lifes[id];
                    __instance.OnSpell = true;
                    __instance.Time = 102;
                    __instance.HealthPoint = 0;
                }
                else
                {
                    __instance.Life = lifes[id];
                    __instance.OnSpell = (onspells[id] == 1);
                    if (set_life[id] == 1)
                    {
                        __instance.HealthPoint = __instance.SpellcardHP;
                    }
                }

            }
        }
        public static void BossJump_FSC(BaseEffect __instance)
        {
            if (__instance.Time < __instance.LifeTime - 20)
                __instance.Time = __instance.LifeTime - 20;
        }
        public static void JumpStory(BaseStory __instance)
        {
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
        }

        public static void BeginBoss(IGameState __instance,int time2, int startTime,Action BGM_Change)
        {
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
            int time = (int)(f_timeMain.GetValue(__instance, null));
            if (time == time2 - 130)
            {
                f_timeMain.SetValue(__instance, time2 - 1, null);
            }
            else if (time == startTime)//testStartTime
            {
                BGM_Change();
            }
        }
    }
}
