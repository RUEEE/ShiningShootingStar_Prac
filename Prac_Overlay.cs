using HarmonyLib;
using Shooting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace SSS_Prac_Launcher
{
    public class LocaleName
    {
        static Dictionary<string, string> locale = new Dictionary<string, string> {
            { "Invincable","无敌"},
            { "Disable pause","禁用暂停"},
            { "Disable X","禁用X键"},

            { "Type","类型"},
            { "Phase","阶段"},
            { "Road","道中"},
            { "Boss","Boss"},
            { "MidBoss","道中Boss"},

            { "Life","残"},
            { "LifePeice","残碎"},
            { "LifeCnt","获得残数"},
            { "Bomb","雷"},
            { "BombPeice","雷碎"},
            { "Border","已开结界数"},
            { "Power","灵力"},
            { "Dian","最大得点"},
            { "Graze","擦弹"},
            { "BorderPercnt","结界百分比"},
            { "Score","分数(无个位)"},

            { "Color","结界颜色"},
            { "Red","红"},
            { "Blue","蓝"},
            { "Green","绿"},

            { "Apply","应用"},
            { "Cancel","取消"},
            
            { "normal 1","~一非~"},
            { "normal 2","~二非~"},
            { "normal 3","~三非~"},
            { "normal 4","~四非~"},
            { "normal 5","~五非~"},
            { "normal 6","~六非~"},
            { "normal 7","~七非~"},
            { "normal 8","~巴菲~"},

            { "card 1","!一符!"},
            { "card 2","!二符!"},
            { "card 3","!三符!"},
            { "card 4","!四符!"},
            { "card 5","!五符!"},
            { "card 6","!六符!"},
            { "card 7","!七符!"},
            { "card 8","!八符!"},
            { "card 9","!九符!"},
            { "card 10","!十符!"},
            { "FSC","FSC"},
            { "FSC 1","FSC 1"},
            { "FSC 2","FSC 2"},
            { "FSC 3","FSC 3"},
        };

        public static void InsertLocaledName(string id,string name,string language = "cn")
        {
            if (locale.ContainsKey(id)) 
                return;
            locale.Add(id, name);
        }

        public static string GetLocaledName(string id)
        {
            if(locale.ContainsKey(id)) 
                return locale[id];
            return id;
        }
        public static string[] GetLocaledNames(string []ids)
        {
            string[] strs = new string[ids.Length];
            for (int i = 0; i<ids.Length; i++)
            {
                strs[i]= GetLocaledName(ids[i]);
            }
            return strs;
        }
    }
    class Prac_Hotkey
    {
        public delegate void HotKeyDelegate(bool val);
        public static List<Label> labels_test = new List<Label>();
        public static List<Prac_Hotkey> hotkeys = new List<Prac_Hotkey>();

        public static Label label_version;
        public static Label label_drama_frame;
        public static Label label_is_prac;

        static int y_overlay = 10;
        static int x_overlay = 10;

        static int y_label = 0;
        static int wait_time_overlay = 0;

        static Panel overlay_panel;

        public static bool is_overlay_panel_open = false;
        public static bool IsOverlayPenelOpen()
        {
            return is_overlay_panel_open;
        }
        public static void CloseOverlayPanel()
        {
            if (IsOverlayPenelOpen())
            {
                if (PatchMainWind.overlay_panel!=null)
                {
                    PatchMainWind.overlay_panel.Controls.Remove(overlay_panel);
                }
                else
                {
                    PatchMainWind.main_form.Controls.Remove(overlay_panel);
                }
            }
            is_overlay_panel_open = false;
            wait_time_overlay = 0;
        }
        public static void OpenOverlayPanel()
        {
            if(!IsOverlayPenelOpen())
            {
                if (PatchMainWind.overlay_panel!=null)
                {
                    PatchMainWind.overlay_panel.Controls.Add(overlay_panel);
                    x_overlay = (int)((float)PatchMainWind.overlay_panel.Width*0.75f);
                    y_overlay = (int)((float)PatchMainWind.overlay_panel.Height*0.6f);
                }
                else
                {
                    PatchMainWind.main_form.Controls.Add(overlay_panel);
                    x_overlay = (int)((float)PatchMainWind.main_form.Width * 0.7f);
                    y_overlay = (int)((float)PatchMainWind.main_form.Height * 0.6f);
                }
            }
            overlay_panel.Location = new Point(x_overlay, y_overlay);
            overlay_panel.Padding = new Padding(3, 3, 3, 3);
            is_overlay_panel_open = true;
            wait_time_overlay = 0;
        }
        public static void Init()
        {
            if (PatchMainWind.overlay_panel != null)
            {
                x_overlay = (int)((float)PatchMainWind.overlay_panel.Width*0.75f);
                y_overlay = (int)((float)PatchMainWind.overlay_panel.Height*0.6f);
            }
            else
            {
                x_overlay = (int)((float)PatchMainWind.main_form.Width*0.75f);
                y_overlay = (int)((float)PatchMainWind.main_form.Height*0.6f);
            }
            {
                label_version = PracSelection.GetDefaultLabel($"");
                label_version.Text=$"version: {Application.ProductVersion}";
                label_version.Location = new Point(0, y_label);
                label_version.AutoSize = true;
                label_version.ForeColor = Color.AliceBlue;
                y_label += PatchMainWind.form_font_regular.Height + 3;
                labels_test.Add(label_version);

                // label_drama_frame = PracSelection.GetDefaultLabel("frame: 0");
                // label_drama_frame.Location = new Point(0, y_label);
                // label_drama_frame.AutoSize = true;
                // label_drama_frame.ForeColor = Color.Wheat;
                // y_label += PatchMainWind.form_font_regular.Height  + 3;
                // labels_test.Add(label_drama_frame);

                label_is_prac = PracSelection.GetDefaultLabel($"is_prac: {PracSelection.is_Prac}");
                label_is_prac.Location = new Point(0, y_label);
                label_is_prac.AutoSize = true;
                label_is_prac.ForeColor = Color.Wheat;
                y_label += PatchMainWind.form_font_regular.Height + 3;
                labels_test.Add(label_is_prac);
                y_label += 5;
            }
            hotkeys.Add(new Prac_Hotkey(SlimDX.DirectInput.Key.F1,Keys.F1, "F1", "Invincable", (bool s) => { OverLayPatches.is_invincable = s; }, Color.Snow));
            hotkeys.Add(new Prac_Hotkey(SlimDX.DirectInput.Key.F2,Keys.F2, "F2", "Disable X", (bool s) => { OverLayPatches.is_disable_X = s; }, Color.Snow));
            hotkeys.Add(new Prac_Hotkey(SlimDX.DirectInput.Key.F3,Keys.F3, "F3", "Disable pause", (bool s) => { OverLayPatches.is_disable_pause = s; }, Color.Snow));

            overlay_panel = new Panel();
            overlay_panel.Location = new Point(x_overlay, y_overlay);
            overlay_panel.BackColor = Color.MidnightBlue;
            overlay_panel.AutoSize = true;
            overlay_panel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            overlay_panel.ForeColor = Color.MidnightBlue;

            foreach (var hotkey in hotkeys)
            {
                overlay_panel.Controls.Add(hotkey.label);
            }
            foreach (var lbs in labels_test)
            {
                overlay_panel.Controls.Add(lbs);
            }

            // dinput
            PatchInput.actions_input_patch += () => {
                if (wait_time_overlay >= 10 &&  PatchInput.key_state!=null && PatchInput.key_state.IsPressed(SlimDX.DirectInput.Key.Backspace)){
                    if(! (PracSelection.IsSelectionPanelOpen() && (PracSelection.objs_selection[PracSelection.cur_selected_option] is NumericUpDown)) )
                    {
                        if (!IsOverlayPenelOpen())
                            OpenOverlayPanel();
                        else
                            CloseOverlayPanel();
                    }
                }
                wait_time_overlay++;
                if(IsOverlayPenelOpen()) {
                    label_is_prac.Text = $"is_prac: {PracSelection.is_Prac}";
                }
            };
            // no dinput
            // PatchMainWind.main_form.KeyDown += (object sender, KeyEventArgs e) =>
            // {
            //     if (wait_time_overlay >= 10 && e.KeyCode == Keys.Back)
            //     {
            //         overlay_panel.Visible  = !overlay_panel.Visible;
            //         wait_time_overlay = 0;
            //     }
            // };
        }

        Label label;
        HotKeyDelegate hotKeyDelegate;
        public bool isActivated = false;
        private SlimDX.DirectInput.Key hotKey_dinput;
        private Keys hotKey_win32;
        private string name;
        private string keyName;
        private int wait_time = 0;
        public Prac_Hotkey(SlimDX.DirectInput.Key keyCode,Keys keyCode2, string keyName, string name, HotKeyDelegate d, Color color)
        {
            this.name = name;
            this.keyName = keyName;
            this.hotKey_win32 = keyCode2;

            label = new Label();
            label.Text = $"[{keyName}] {LocaleName.GetLocaledName(name)}";
            label.BackColor = Color.Transparent;
            label.ForeColor = color;
            label.Font = PatchMainWind.form_font_regular;
            label.Location = new Point(0, y_label);
            label.AutoSize = true;

            hotKeyDelegate = d;
            hotKey_dinput = keyCode;
            y_label += PatchMainWind.form_font_regular.Height + 3;

            // dinput used
            PatchInput.actions_input_patch += () => {
                if (wait_time>=10 &&  PatchInput.key_state!=null && PatchInput.key_state.IsPressed(keyCode) && label.Visible)
                {
                    isActivated = !isActivated;
                    wait_time = 0;
                    hotKeyDelegate(isActivated);
                    label.ForeColor = isActivated?Color.Lime : color;
                    label.Font = isActivated ? PatchMainWind.form_font_italian: PatchMainWind.form_font_regular;
                }
                wait_time++;
            };
            // no dinput
            // PatchMainWind.main_form.KeyDown += (object sender, KeyEventArgs e) =>
            // {
            //     if (wait_time >= 10 && e.KeyCode == hotKey_win32 && label.Visible)
            //     {
            //         isActivated = !isActivated;
            //         wait_time = 0;
            //         hotKeyDelegate(isActivated);
            //         label.ForeColor = isActivated ? Color.LimeGreen : color;
            //         label.Font = isActivated ? PatchMainWind.form_font_italian : PatchMainWind.form_font_regular;
            //     }
            // };
        }
    }

    [HarmonyPatch]
    public class OverLayPatches
    {
        public static bool is_invincable = false;
        public static bool is_disable_X = false;
        public static bool is_disable_pause = false;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Shooting.BaseMyPlane), "PreMiss")]
        public static void BaseMyPlane_PreMiss_Invincable_Prefix(BaseMyPlane __instance)
        {

            if (is_invincable)
                __instance.DeadTime = -1;
            return;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Shooting.MyPlane_Koishi), "PreMiss")]
        public static void MyPlane_Koishi_PreMiss_Invincable_Prefix(BaseMyPlane __instance)
        {

            if (is_invincable)
                __instance.DeadTime = -1;
            return;
        }

    }
    [HarmonyPatch]
    class PatchDisablePause
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Shooting.Game_Main:Form_Main_Deactivate");
        }
        public static bool Prefix(Shooting.Game_Main __instance, object sender, EventArgs e)
        {

            if (OverLayPatches.is_disable_pause)
                return false;
            return true;
        }
    }
}
