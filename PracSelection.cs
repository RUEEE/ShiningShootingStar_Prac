using HarmonyLib;
using Shooting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace SSS_Prac_Launcher
{
    public class PracSelection
    {
        public static bool is_Prac = false;

        public static TableLayoutPanel selection_panel;
        public static Shooting.BaseMenuGroup menuGroup;

        public static ComboBox comboBox_stage_sel;
        public static ComboBox comboBox_type_sel;

        public static ComboBox comboBox_subStage_sel;

        static Type type_MenuGroup_PlayerSelect = AccessTools.TypeByName("Shooting.MenuGroup_PlayerSelect");

        public static int tab_id = 0;
        public static Label GetDefaultLabel(string text)
        {
            
            Label lb = new Label();
            lb.Text = text;
            lb.Font = PatchMainWind.form_font_regular;
            lb.ForeColor = Color.Black;
            lb.BackColor = Color.Transparent;
            return lb;
        }
        public static ComboBox GetDefaultCombobox(object[] items)
        {
            ComboBox combo = new ComboBox();
            combo.Items.AddRange(items);
            combo.SelectedIndex = 0;
            combo.TabIndex = tab_id;
            tab_id++;
            combo.Font = PatchMainWind.form_font_regular;
            combo.ForeColor = Color.Black;
            combo.BackColor = Color.White;
            combo.TabStop = true;
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            
            return combo;
        }
        public static void Init()
        {
            selection_panel = new TableLayoutPanel();
            selection_panel.AutoSize=true;
            selection_panel.AutoSizeMode=AutoSizeMode.GrowAndShrink;
            selection_panel.Visible = false;
            selection_panel.ForeColor = Color.AntiqueWhite;
            selection_panel.BackColor = Color.AntiqueWhite;
            selection_panel.Padding = new Padding(10, 10, 10, 10);

            if(PatchMainWind.main_panel!=null)
            {
                PatchMainWind.main_panel.Controls.Add(selection_panel);
            }else {
                PatchMainWind.main_form.Controls.Add(selection_panel);
            }

            selection_panel.ColumnCount = 2;
            selection_panel.RowCount = 4;

            int row = 0;
            {
                ComboBox combo = GetDefaultCombobox(new Object[] { "1", "2", "3", "4", "5", "6", "ex" });
                combo.SelectedIndexChanged += ComboChange_Stage_Type;

                selection_panel.Controls.Add(GetDefaultLabel("Stage"), 0, row);
                selection_panel.Controls.Add(combo, 1, row);
                comboBox_stage_sel = combo;
                row++;
            }
            {
                ComboBox combo = GetDefaultCombobox(new Object[] { "Road", "Boss" });
                combo.SelectedIndexChanged += ComboChange_Stage_Type;

                selection_panel.Controls.Add(GetDefaultLabel("Type"), 0, row);
                selection_panel.Controls.Add(combo, 1, row);
                comboBox_type_sel = combo;
                row++;
            }
            {
                ComboBox combo = GetDefaultCombobox(new Object[] { " " });

                selection_panel.Controls.Add(GetDefaultLabel("Phase"), 0, row);
                selection_panel.Controls.Add(combo, 1, row);
                comboBox_subStage_sel = combo;
                ComboChange_Stage_Type(null, null);// init this
                row++;
            }

            Update();
            PatchRender.actions_render_patch += Update;
        }

        public static void ComboChange_Stage_Type(object sender, EventArgs e)
        {
            if (comboBox_type_sel==null)
                return;
            comboBox_subStage_sel.Items.Clear();
            if (comboBox_type_sel.SelectedIndex == 1)//boss
            {
                comboBox_subStage_sel.Items.AddRange(new object[] {"normal 1","card 1","normal 2","card 2","normal 3","card 3","normal 4","card 4","card 5","card 6","LSC 1","LSC 2","LSC 3" });
            }
            else //road
            {
                comboBox_subStage_sel.Items.AddRange(new object[] { "road 1" });
            }
            comboBox_subStage_sel.SelectedIndex = 0;
        }
        public static void Update()
        {
            selection_panel.Location = new Point(PatchMainWind.main_form.Width/2-selection_panel.Width/2, PatchMainWind.main_form.Height/2-selection_panel.Height/2);
            if(selection_panel.Visible == true)
            {
                int cnt = menuGroup.StageData.MenuGroupList.Count;
                if (cnt!=0 && menuGroup.StageData.MenuGroupList[cnt-1].GetType() != type_MenuGroup_PlayerSelect)
                {
                    selection_panel.Visible = false;
                }
            }
        }

        static public void EnterStage()
        {
            is_Prac = true;
            PracSelection.selection_panel.Visible = false;

            Point originalPosition = new Point(192, 398);
            switch (PracSelection.menuGroup.MenuItemList[PracSelection.menuGroup.MenuSelectIndex].Name)
            {
                case "FaceReimu_me":
                     PracSelection.menuGroup.StageData.StateSwitchData.SDPswitch.MyPlane = new MyPlane_Reimu(PracSelection.menuGroup.StageData.StateSwitchData.SDPswitch, originalPosition);
                    break;
                case "FaceSanae_me":
                    PracSelection.menuGroup.StageData.StateSwitchData.SDPswitch.MyPlane = new MyPlane_Sanae(PracSelection.menuGroup.StageData.StateSwitchData.SDPswitch, originalPosition);
                    break;
                case "FaceMarisa_me":
                    PracSelection.menuGroup.StageData.StateSwitchData.SDPswitch.MyPlane = new MyPlane_Marisa(PracSelection.menuGroup.StageData.StateSwitchData.SDPswitch, originalPosition);
                    break;
                case "FaceKoishi_me":
                    PracSelection.menuGroup.StageData.StateSwitchData.SDPswitch.MyPlane = new MyPlane_Koishi(PracSelection.menuGroup.StageData.StateSwitchData.SDPswitch, originalPosition);
                    break;
                case "FaceAya_ct":
                    PracSelection.menuGroup.StageData.StateSwitchData.SDPswitch.MyPlane = new MyPlane_Aya(PracSelection.menuGroup.StageData.StateSwitchData.SDPswitch, originalPosition);
                    break;
            }

            PracSelection.menuGroup.StageData.StateSwitchData.SDPswitch.MyPlane.Power = 400;
            PracSelection.menuGroup.StageData.Rep.CreatRpy();


            if (PracSelection.comboBox_stage_sel.SelectedIndex!=6)//not ex
            {
                if (PracSelection.comboBox_type_sel.SelectedIndex==0)//stage
                {
                    PracSelection.menuGroup.StageData.StateSwitchData.NextState = $"St{PracSelection.comboBox_stage_sel.SelectedIndex + 1}";
                }
                else
                {
                    PracSelection.menuGroup.StageData.StateSwitchData.NextState = $"Bs{PracSelection.comboBox_stage_sel.SelectedIndex + 1}";
                }
            }else
            {
                PracSelection.menuGroup.StageData.StateSwitchData.NextState = $"StEx";
            }
            PracSelection.menuGroup.StageData.StateSwitchData.SDPswitch.SetReplayInfo(PracSelection.menuGroup.StageData.StateSwitchData.NextState);
            return;
        }
    }


    [HarmonyPatch]
    public class PatchPlayerSelectionZ
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Shooting.MenuGroup_PlayerSelect:ProcessZ");
        }
        public static bool Prefix(Shooting.BaseMenuGroup __instance)
        {
            var type = AccessTools.GetDeclaredFields(AccessTools.TypeByName("Shooting.MenuGroup_PlayerSelect"));
            FieldInfo ff = null;
            foreach (var field in type)
            {
                if (field.Name=="StageSelect")
                {
                    ff = field;
                    break;
                }
            }
            if (ff == null)
                return true;
            bool stageselect = (bool)ff.GetValue(__instance);
            if (!stageselect)
            {
                return true;
            }

            PracSelection.menuGroup = __instance;
            if (PracSelection.selection_panel.Visible==false)
            {
                PracSelection.selection_panel.Visible = true;
            }
            else {
                PracSelection.EnterStage();
            }
            return false;
        }
    }


    [HarmonyPatch]
    public class PatchPlayerSelectionX
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Shooting.MenuGroup_PlayerSelect:ProcessKeys");
        }
        public static bool Prefix(Shooting.BaseMenuGroup __instance)
        {
            if (PracSelection.selection_panel.Visible == false)
            {
                return true;
            }
            if ((__instance.KClass.Key_X || __instance.KClass.Key_ESC) && __instance.LastX == 0)
            {
                PracSelection.selection_panel.Visible = false;
                __instance.StageData.SoundPlay("se_cancel00.wav");
                return false;
            }
            return true;
        }
    }
}
