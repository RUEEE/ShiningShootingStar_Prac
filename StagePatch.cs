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
        public const int NO_FSC = -2;
        public static void BossJump(BaseBoss __instance,int[] lifes, int[] set_life, int[] onspells,int FSC = -1)
        {
            if (FSC == -1)
                FSC = lifes.Length - 1;
            int id = PracSelection.comboBox_subStage_sel.SelectedIndex;
            if (id <= lifes.Length && id >= 0)
            {
                if (FSC != NO_FSC && id >= FSC)//FSC
                {
                    __instance.Life = lifes[id];
                    __instance.OnSpell = true;
                    __instance.Time = 102;
                    __instance.HealthPoint = 0;
                }
                else
                {
                    __instance.Life = lifes[id];
                    
                    if (set_life[id] == 1)
                    {
                        __instance.HealthPoint = __instance.SpellcardHP;
                        __instance.OnSpell = (onspells[id] == 1);
                    }
                    else
                    {
                        if (onspells[id] == 1)
                        {
                            //set_life==0 且 onspells==1 的情况下, 符卡独占一条血, 这个时候游戏在 T=100 时设置符卡时间
                            __instance.OnSpell = false;
                            __instance.Time = 99;
                        }
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


        static IGameState last_inst;
        static PropertyInfo last_info_p_timeMain;
        public static PropertyInfo GetTimeMain(IGameState __instance)
        {
            if (__instance == last_inst && last_info_p_timeMain != null)// reflection is slow
                return last_info_p_timeMain;
            last_inst = __instance;
            var type = AccessTools.GetDeclaredProperties(AccessTools.TypeByName("Shooting.BaseGameState"));
            PropertyInfo p_timeMain = null;
            foreach (var prop in type)
            {
                if (prop.Name == "TimeMain")
                {
                    p_timeMain = prop;
                    break;
                }
            }
            last_info_p_timeMain = p_timeMain;
            return p_timeMain;
        }
        public static void RecordTimes(IGameState __instance, PropertyInfo p_timeMain)
        {
            return;
            Prac_Hotkey.label_drama_frame.Text=$"frame: {(int)(p_timeMain.GetValue(__instance, null))}";
        }

        public static void RecordTimes(IGameState __instance)
        {
            return;
            var t = GetTimeMain(__instance);
            if(t != null)
                Prac_Hotkey.label_drama_frame.Text = $"frame: {(int)(GetTimeMain(__instance).GetValue(__instance, null))}";
        }
        public static void RoadJump(IGameState __instance, PropertyInfo p_timeMain)
        {
            int time = (int)(p_timeMain.GetValue(__instance, null));
            int stage = PracSelection.comboBox_stage_sel.SelectedIndex;
            int time_later = PracSelection.time_later[stage];

            if (time == 2)
            {
                int road_idx = PracSelection.comboBox_subStage_sel.SelectedIndex;
                int time_to_jump = PracSelection.road_def[stage][road_idx].time;
                if (PracSelection.road_def[stage][road_idx].is_later)
                {
                    __instance.StageData.EnemyPlaneList.Clear();
                    p_timeMain.SetValue(__instance, time_later, null);
                }
                else
                {
                    if (time_to_jump > 0 && time < time_to_jump)
                    {
                        p_timeMain.SetValue(__instance, time_to_jump - 1, null);
                        foreach (var enm in __instance.StageData.EnemyPlaneList)
                        {
                            enm.Time = enm.Time + (time_to_jump - 2);
                        }
                    }
                }
            }else if(time == time_later + 1)// from base.TimeMain > array[1]
            {
                int road_idx = PracSelection.comboBox_subStage_sel.SelectedIndex;
                int time_to_jump = PracSelection.road_def[stage][road_idx].time;
                if (time_to_jump > 0 && time < time_to_jump)
                {
                    p_timeMain.SetValue(__instance, time_to_jump - 1, null);
                    foreach (var enm in __instance.StageData.EnemyPlaneList)
                    {
                        enm.Time = enm.Time + (time_to_jump - 2) - time_later;
                    }
                }
            }
        }
        public static void BeginBoss(IGameState __instance, PropertyInfo p_timeMain, int time2, int startTime,Action BGM_Change)
        {
            int time = (int)(p_timeMain.GetValue(__instance, null));
            if (time == time2 - 130)
            {
                p_timeMain.SetValue(__instance, time2 - 1, null);
            }
            else if (time == startTime)//testStartTime
            {
                BGM_Change();
            }
        }
        public static void BeginMidBoss(IGameState __instance, PropertyInfo p_timeMain, int time0)
        {
            int time = (int)(p_timeMain.GetValue(__instance, null));
            if (time == 2)
            {
                p_timeMain.SetValue(__instance, time0 - 1, null);
            }
        }

        public static void SetFSC_EnhanceCount(IGameState __instance, PropertyInfo p_timeMain)
        {
            int time = (int)(p_timeMain.GetValue(__instance, null));
            if (time == 1)
            {
                int stage = PracSelection.comboBox_stage_sel.SelectedIndex;
                if (stage>=0 && stage<=6 && PracSelection.comboBox_subStage_sel.SelectedIndex == PracSelection.n_FSC[stage])
                {
                    var type = AccessTools.GetDeclaredProperties(AccessTools.TypeByName("Shooting.BaseGameState"));
                    PropertyInfo p_base_my_plane = null;
                    foreach (var prop in type)
                    {
                        if (prop.Name == "MyPlane")
                        {
                            p_base_my_plane = prop;
                            break;
                        }
                    }
                    if (p_base_my_plane != null)
                    {
                        var bmp = (BaseMyPlane)p_base_my_plane.GetValue(__instance, null);
                        bmp.EnchantmentCount = bmp.EnchantmentCountNeeded;
                    }
                }
                
            }
        }
    }
}
