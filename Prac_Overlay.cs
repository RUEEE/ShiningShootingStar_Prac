using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace SSS_Prac_Launcher
{
    class Prac_Hotkey
    {
        public delegate void HotKeyDelegate(bool val);
        public static List<Prac_Hotkey> hotkeys = new List<Prac_Hotkey>();

        static int y_overlay = 10;
        static int x_overlay = 10;

        static int y_label = 0;
        static int wait_time_overlay = 0;
        static bool show_overlay = false;

        static Panel overlay_panel;
        public static void Init()
        {
            hotkeys.Add(new Prac_Hotkey(SlimDX.DirectInput.Key.F1, "F1", "invincable", (bool s) => { OverLayPatches.is_invincable = s; }, Color.White));
            hotkeys.Add(new Prac_Hotkey(SlimDX.DirectInput.Key.F2, "F2", "test", (bool s) => { }, Color.White));

            overlay_panel = new Panel();
            overlay_panel.Location = new Point(x_overlay, y_overlay);
            overlay_panel.BackColor = Color.Black;
            overlay_panel.AutoSize = true;
            overlay_panel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            overlay_panel.ForeColor = Color.Black;
            overlay_panel.Visible = show_overlay;

            if (PatchMainWind.main_panel!=null)
            {
                PatchMainWind.main_panel.Controls.Add(overlay_panel);
            }
            else
            {
                PatchMainWind.main_form.Controls.Add(overlay_panel);
            }
            foreach (var hotkey in hotkeys)
            {
                overlay_panel.Controls.Add(hotkey.label);
            }

            PatchRender.actions_render_patch += () => {
                if (wait_time_overlay >= 10 &&  PatchKeyDown.key_state!=null && PatchKeyDown.key_state.IsPressed(SlimDX.DirectInput.Key.Backspace)){
                    wait_time_overlay = 0;
                    show_overlay = !show_overlay;
                    overlay_panel.Visible = show_overlay;
                }
                wait_time_overlay++;
            };
        }

        Label label;
        HotKeyDelegate hotKeyDelegate;
        public bool isActivated = false;
        private SlimDX.DirectInput.Key hotKey;
        private string name;
        private string keyName;
        private int wait_time = 0;
        public Prac_Hotkey(SlimDX.DirectInput.Key keyCode, string keyName, string name, HotKeyDelegate d, Color color)
        {
            this.name = name;
            this.keyName = keyName;

            label = new Label();
            label.Text = $"[{keyName}] {name}";
            label.BackColor = Color.Transparent;
            label.ForeColor = color;
            label.Font = PatchMainWind.form_font_regular;
            label.Location = new Point(0, y_label);
            label.AutoSize = true;

            hotKeyDelegate = d;
            hotKey = keyCode;
            y_label += PatchMainWind.form_font_regular.Height;

            PatchRender.actions_render_patch += () => {
                if (wait_time>=10 &&  PatchKeyDown.key_state!=null && PatchKeyDown.key_state.IsPressed(keyCode) && label.Visible)
                {
                    isActivated = !isActivated;
                    wait_time = 0;
                    hotKeyDelegate(isActivated);
                    label.ForeColor = isActivated?Color.LimeGreen:color;
                    label.Font = isActivated ? PatchMainWind.form_font_italian: PatchMainWind.form_font_regular;
                }
                wait_time++;
            };
        }
    }

    public class OverLayPatches
    {
        public static bool is_invincable = false;
    }
}
