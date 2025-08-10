using HarmonyLib;
using Shooting;
using SlimDX;
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

        public static int[] n_FSC = new int[] { 4, 5, 6, 7, 8, 10, 18 };
        public static int[] time_later = new int[] { 4000, 5000, 3800, 9999999, 3000, 999999, 5060 };
        public static int[] lifepeice_cnts = new int[] { 3, 5, 8, 10, 12, 15, 18, 20, 22 };
        public enum SelectedType {
            Road = 0,Boss=1,MidBoss=2
        }
        public static bool is_Prac = false;

        public static TableLayoutPanel selection_panel;
        public static Shooting.BaseMenuGroup menuGroup;

        public static ComboBox comboBox_stage_sel;
        public static ComboBox comboBox_type_sel;
        public static ComboBox comboBox_subStage_sel;

        public static NumericUpDown numBox_life;
        public static NumericUpDown numBox_lifepeice;
        public static NumericUpDown numBox_lifeget_cnt;

        public static NumericUpDown numBox_bomb;
        public static NumericUpDown numBox_bombpeice;
        
        public static NumericUpDown numBox_power;
        public static NumericUpDown numBox_dian;
        public static NumericUpDown numBox_graze;
        
        public static NumericUpDown numBox_score;
        
        public static NumericUpDown numBox_border_percent;
        public static ComboBox comboBox_color;

        public static Button btn_apply;
        public static Button btn_cancel;

        public static Label lb_lifepeice;

        static Type type_MenuGroup_PlayerSelect = AccessTools.TypeByName("Shooting.MenuGroup_PlayerSelect");

        public static int tab_id = 0;

        public static List<Label> lbs_selection  = new List<Label>();
        public static List<Control> objs_selection = new List<Control>();
        public static int cur_selected_option = 0;

        public static void SelectionPanelOpen()
        {
            if (!IsSelectionPanelOpen())
            {
                if (PatchMainWind.main_panel != null)
                {
                    PatchMainWind.main_panel.Controls.Add(selection_panel);
                    selection_panel.Location = new Point(PatchMainWind.main_panel.Width/2-selection_panel.Width/2, PatchMainWind.main_panel.Height/2-selection_panel.Height/2);
                }
                else
                {
                    PatchMainWind.main_form.Controls.Add(selection_panel);
                    selection_panel.Location = new Point(PatchMainWind.main_form.Width/2-selection_panel.Width/2, PatchMainWind.main_form.Height/2-selection_panel.Height/2);
                }
            }
            key_wait_time = 0;
            cur_selected_option = 0;
            UpdateSelectedOption();
        }
        public static void SelectionPanelClose()
        {
            if (IsSelectionPanelOpen())
            {
                if (PatchMainWind.main_panel!=null)
                {
                    PatchMainWind.main_panel.Controls.Remove(selection_panel);
                    PatchMainWind.main_panel.Select();
                    PatchMainWind.main_panel.Focus();
                }
                else
                {
                    PatchMainWind.main_form.Controls.Remove(selection_panel);
                    PatchMainWind.main_form.Select();
                    PatchMainWind.main_form.Focus();
                }
            }
            key_wait_time = 0;
        }

        public static bool IsSelectionPanelOpen()
        {
            if (PatchMainWind.main_panel!=null)
            {
                return PatchMainWind.main_panel.Controls.Contains(selection_panel);
            }
            return PatchMainWind.main_form.Controls.Contains(selection_panel);
        }

        public struct RoadJump
        {
            public int time;
            public bool is_later;
            public string id_name;
            public RoadJump(int t,bool is_later_road, string id) { time=t; is_later = is_later_road; id_name=id;}
        };

        public static List<RoadJump>[] road_def = new List<RoadJump>[7]; // comboboxIdx -> idStr


        public static void InsertRoadDef(int stageIdx,int time, bool is_later_road, string id_name,string localed_name)
        {
            road_def[stageIdx].Add(new RoadJump(time, is_later_road, id_name));
            LocaleName.InsertLocaledName(id_name, localed_name);
        }
        public static void InitRoadDefine()
        {
            for(int i=0;i<road_def.Length;i++)
                road_def[i] = new List<RoadJump>();
            InsertRoadDef(0, -1,    false, "road 1-0", "默认1面");
            InsertRoadDef(0, 0,     false, "road 1-1", "1-1 一开");
            InsertRoadDef(0, 5260,  true,  "road 1-2", "1-2 一后半");
                                  
            InsertRoadDef(1, -1,    false, "road 2-0", "默认2面");
            InsertRoadDef(1, 0,     false, "road 2-1", "2-1 二开");
            InsertRoadDef(1, 1450,  false, "road 2-2", "2-2 二标题后");
            InsertRoadDef(1, 3000,  false, "road 2-3", "2-3 魔神大蝴蝶");
            InsertRoadDef(1, 6000,  true,  "road 2-4", "2-4 二后半");

                                   
            InsertRoadDef(2, -1,    false, "road 3-0", "默认3面");
            InsertRoadDef(2, 0,     false, "road 3-1", "3-1 三开");
            InsertRoadDef(2, 850,   false, "road 3-2", "3-2 三标题后");
            InsertRoadDef(2, 1690,  false, "road 3-3", "3-3 吴克");
            InsertRoadDef(2, 5050,  true,  "road 3-4", "3-4 三后半开");
            InsertRoadDef(2, 5440,  true,  "road 3-5", "3-3 毛玉阵");
            InsertRoadDef(2, 5950,  true,  "road 3-6", "3-6 三闭幕");
                                  
            InsertRoadDef(3, -1,    false, "road 4-0", "默认4面");
            InsertRoadDef(3, 0,     false, "road 4-1", "4-1 四开");
            InsertRoadDef(3, 720,   false, "road 4-2", "4-2 镜子大蝴蝶");
            InsertRoadDef(3, 2380,  false, "road 4-3", "4-3 自机狙妖精与大蝴蝶");
            InsertRoadDef(3, 3880,  false, "road 4-4", "4-4 自机狙阵");
            InsertRoadDef(3, 6600,  false, "road 4-5", "4-5 大瀑布");
                                   
            InsertRoadDef(4, -1,    false, "road 5-0", "默认5面");
            InsertRoadDef(4, 400,   false, "road 5-1", "5-1 五开");
            InsertRoadDef(4, 800,   false, "road 5-2", "5-2 低处大蝴蝶");
            InsertRoadDef(4, 1600,  false, "road 5-3", "5-3 高处大蝴蝶");
            InsertRoadDef(4, 1920,  false, "road 5-4", "5-4 前半吴克");
            InsertRoadDef(4, 4090,  true,  "road 5-5", "5-5 五后半开");
            InsertRoadDef(4, 5700,  true,  "road 5-6", "5-6 后半飞行阵");
            InsertRoadDef(4, 7400,  true,  "road 5-7", "5-7 后闭幕");
                                    
            InsertRoadDef(5, -1,    false, "road 6-0", "默认6面");
            InsertRoadDef(5, 600,   false, "road 6-1", "6-1 六开");
            InsertRoadDef(5, 2060,  false, "road 6-2", "6-2 六标题后");
            InsertRoadDef(5, 3500,  false, "road 6-3", "6-3 牙签阵");
            InsertRoadDef(5, 4950,  false, "road 6-4", "6-4 飞行大蝴蝶");
            InsertRoadDef(5, 6400,  false, "road 6-5", "6-5 狗运流星雨");

            InsertRoadDef(6, -1,     false, "road 7-0", "默认Ex面");
            InsertRoadDef(6, 0,      false, "road 7-1", "7-1 EX开");
            InsertRoadDef(6, 6500,   true,  "road 7-12", "7-2 EX后半");
        }


        public static void ComboSetItems(ComboBox cb, string[] items)
        {
            int idx = cb.SelectedIndex;
            cb.Items.Clear();
            cb.Items.AddRange(LocaleName.GetLocaledNames(items));
            if(idx < items.Length)
                cb.SelectedIndex = idx;
            else
                cb.SelectedIndex = 0;
        }

        public static NumericUpDown GetDefaultNumBox(int min,int max,int numdefault)
        {
            NumericUpDown nb = new NumericUpDown();
            nb.TabIndex = tab_id;
            tab_id++;
            nb.TabStop = false;
            nb.Font = PatchMainWind.form_font_regular;
            nb.ForeColor = Color.Black;
            nb.BackColor = Color.White;
            nb.ClientSize = new Size(nb.ClientSize.Height * 10, nb.ClientSize.Height);

            nb.Minimum = min;
            nb.Maximum = max;
            nb.Value = numdefault;
            nb.InterceptArrowKeys = false;
            return nb;
        }
        public static Button GetDefaultBtn(string text)
        {
            Button btn = new Button();
            btn.TabIndex = tab_id;
            btn.TabStop = false;
            tab_id++;
            btn.Text = LocaleName.GetLocaledName(text);
            btn.Font = PatchMainWind.form_font_regular;
            btn.ForeColor = Color.Black;
            btn.BackColor = Color.White;
            btn.AutoSize = true;
            return btn;
        }
        public static Label GetDefaultLabel(string text)
        {
            
            Label lb = new Label();
            lb.Text = LocaleName.GetLocaledName(text);
            lb.Font = PatchMainWind.form_font_regular;
            lb.ForeColor = Color.Black;
            lb.BackColor = Color.Transparent;
            lb.AutoSize = true;
            return lb;
        }
        public static ComboBox GetDefaultCombobox(string[] items)
        {
            ComboBox combo = new ComboBox();
            ComboSetItems(combo, items);
            combo.SelectedIndex = 0;
            combo.TabIndex = tab_id;
            tab_id++;
            combo.TabStop = false;
            combo.Font = PatchMainWind.form_font_regular;
            combo.ForeColor = Color.Black;
            combo.BackColor = Color.White;
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.ClientSize = new Size(combo.ClientSize.Height * 12, combo.ClientSize.Height);
            combo.KeyDown += (o, e) =>
            {
                if (e.KeyData==Keys.Up || e.KeyData==Keys.Down)
                    e.Handled = true;
                else
                    e.Handled = false;
            };
            combo.KeyUp+=(o, e) =>
            {
                if (e.KeyData==Keys.Up || e.KeyData==Keys.Down)
                    e.Handled = true;
                else
                    e.Handled = false;
            };
            return combo;
        }
        public static void InitValues(BaseMyPlane plane)
        {
            plane.Power = (int)numBox_power.Value;
            plane.Life = (int)numBox_life.Value;
            plane.Spell = (int)numBox_bomb.Value;
            plane.LifeChip = (int)numBox_lifepeice.Value;
            plane.SpellChip = (int)numBox_bombpeice.Value;
            plane.LifeUpCount = (int)numBox_lifeget_cnt.Value;
            plane.Score = (long)numBox_score.Value * 10;
            plane.HighItemScore = (int)numBox_dian.Value;
            plane.Graze = (int)numBox_graze.Value;
            plane.StarPoint = (int)((float)numBox_border_percent.Value/100.0f*3000.0f);
            plane.LastColor = (EnchantmentType)(comboBox_color.SelectedIndex + 1);
        }
        public static void Init()
        {
            InitRoadDefine();

            selection_panel = new TableLayoutPanel();
            selection_panel.AutoSize=true;
            selection_panel.AutoSizeMode=AutoSizeMode.GrowAndShrink;
            selection_panel.ForeColor = Color.AntiqueWhite;
            selection_panel.BackColor = Color.AntiqueWhite;
            selection_panel.Padding = new Padding(10, 10, 10, 10);

            selection_panel.ColumnCount = 2;
            selection_panel.RowCount = 10;

            PatchMainWind.main_form.KeyDown += (o, e) =>
            {
                if (((int)e.KeyData>='0' && (int)e.KeyData<='9') || e.KeyData == Keys.Back || e.KeyData==Keys.Right || e.KeyData==Keys.Left)
                    e.Handled = false;
                else
                {
                    if (e.KeyData==Keys.Z && objs_selection[cur_selected_option] is Button)
                        e.Handled = false;
                    else
                        e.Handled = true;
                }
            };
            PatchMainWind.main_form.KeyUp += (o, e) =>
            {
                if (((int)e.KeyData>='0' && (int)e.KeyData<='9') || e.KeyData == Keys.Back || e.KeyData==Keys.Right || e.KeyData==Keys.Left)
                    e.Handled = false;
                else
                {
                    if (e.KeyData==Keys.Z && objs_selection[cur_selected_option] is Button)
                        e.Handled = false;
                    else
                        e.Handled = true;
                }
            };
            PatchMainWind.main_form.KeyPress += (o, e) =>
            {
                if ((e.KeyChar>='0' && e.KeyChar<='9'))
                    e.Handled = false;
                else
                    e.Handled = true;
            };

            int row = 0;
            {// 0
                ComboBox combo = GetDefaultCombobox(new string[] { "stage 1", "stage 2", "stage 3", "stage 4", "stage 5", "stage 6", "stage ex" });
                combo.SelectedIndexChanged += ComboChange_Stage_Type;

                Label lb = GetDefaultLabel("Stage");
                selection_panel.Controls.Add(lb, 0, row);
                selection_panel.Controls.Add(combo, 1, row);
                comboBox_stage_sel = combo;
                lbs_selection.Add(lb);
                objs_selection.Add(combo);
                row++;
            }
            {// 1
                ComboBox combo = GetDefaultCombobox(new string[] { "Road"});
                combo.SelectedIndexChanged += ComboChange_Stage_Type;

                Label lb = GetDefaultLabel("Type");
                selection_panel.Controls.Add(lb, 0, row);
                selection_panel.Controls.Add(combo, 1, row);
                comboBox_type_sel = combo;
                lbs_selection.Add(lb);
                objs_selection.Add(combo);
                row++;
            }
            {// 2
                ComboBox combo = GetDefaultCombobox(new string[] { " " });

                Label lb = GetDefaultLabel("Phase");
                selection_panel.Controls.Add(lb, 0, row);
                selection_panel.Controls.Add(combo, 1, row);
                comboBox_subStage_sel = combo;
                lbs_selection.Add(lb);
                objs_selection.Add(combo);
                row++;
            }
            {
                NumericUpDown nb = GetDefaultNumBox(0, 8, 8);
                Label lb = GetDefaultLabel("Life");
                selection_panel.Controls.Add(lb, 0, row);
                selection_panel.Controls.Add(nb, 1, row);
                numBox_life = nb;
                lbs_selection.Add(lb);
                objs_selection.Add(nb);
                row++;
            }
            {
                NumericUpDown nb = GetDefaultNumBox(0, 3, 0);
                lb_lifepeice = GetDefaultLabel("");
                lb_lifepeice.Text = $"{LocaleName.GetLocaledName("LifePeice")}(0/3)";


                selection_panel.Controls.Add(lb_lifepeice, 0, row);
                selection_panel.Controls.Add(nb, 1, row);
                nb.ValueChanged += (o, e) =>
                {
                    if (numBox_lifeget_cnt != null)
                    {
                        int maxlp = lifepeice_cnts[(int)numBox_lifeget_cnt.Value];
                        lb_lifepeice.Text=$"{LocaleName.GetLocaledName("LifePeice")}({numBox_lifepeice.Value}/{maxlp})";
                    }
                };
                numBox_lifepeice = nb;
                lbs_selection.Add(lb_lifepeice);
                objs_selection.Add(nb);
                row++;
            }
            {
                NumericUpDown nb = GetDefaultNumBox(0, lifepeice_cnts.Length - 1, 0);
                Label lb = GetDefaultLabel("LifeCnt");
                selection_panel.Controls.Add(lb, 0, row);
                selection_panel.Controls.Add(nb, 1, row);
                nb.ValueChanged += (o, e) =>
                {
                    int maxlp = lifepeice_cnts[(int)numBox_lifeget_cnt.Value];
                    numBox_lifepeice.Maximum = maxlp;
                    if (numBox_lifepeice.Value > maxlp)
                        numBox_lifepeice.Value = maxlp;
                    lb_lifepeice.Text=$"{LocaleName.GetLocaledName("LifePeice")}({numBox_lifepeice.Value}/{maxlp})";
                };
                numBox_lifeget_cnt = nb;
                lbs_selection.Add(lb);
                objs_selection.Add(nb);
                row++;
            }
            {
                NumericUpDown nb = GetDefaultNumBox(0, 8, 8);
                Label lb = GetDefaultLabel("Bomb");
                selection_panel.Controls.Add(lb, 0, row);
                selection_panel.Controls.Add(nb, 1, row);
                numBox_bomb = nb;
                lbs_selection.Add(lb);
                objs_selection.Add(nb);
                row++;
            }
            {
                NumericUpDown nb = GetDefaultNumBox(0, 4, 0);
                Label lb = GetDefaultLabel("BombPeice");
                selection_panel.Controls.Add(lb, 0, row);
                selection_panel.Controls.Add(nb, 1, row);
                numBox_bombpeice = nb;
                lbs_selection.Add(lb);
                objs_selection.Add(nb);
                row++;
            }
            {
                NumericUpDown nb = GetDefaultNumBox(100, 400, 400);
                Label lb = GetDefaultLabel("Power");
                selection_panel.Controls.Add(lb, 0, row);
                selection_panel.Controls.Add(nb, 1, row);
                numBox_power = nb;
                lbs_selection.Add(lb);
                objs_selection.Add(nb);
                row++;
            }
            {
                NumericUpDown nb = GetDefaultNumBox(10000, 9999999, 10000);
                Label lb = GetDefaultLabel("Dian");
                selection_panel.Controls.Add(lb, 0, row);
                selection_panel.Controls.Add(nb, 1, row);
                numBox_dian = nb;
                lbs_selection.Add(lb);
                objs_selection.Add(nb);
                row++;
            }
            {
                NumericUpDown nb = GetDefaultNumBox(0, 99999999, 0);
                Label lb = GetDefaultLabel("Graze");
                selection_panel.Controls.Add(lb, 0, row);
                selection_panel.Controls.Add(nb, 1, row);
                numBox_graze = nb;
                lbs_selection.Add(lb);
                objs_selection.Add(nb);
                row++;
            }
            {
                NumericUpDown nb = GetDefaultNumBox(0, 100, 0);
                Label lb = GetDefaultLabel("BorderPercnt");
                selection_panel.Controls.Add(lb, 0, row);
                selection_panel.Controls.Add(nb, 1, row);
                numBox_border_percent = nb;
                lbs_selection.Add(lb);
                objs_selection.Add(nb);
                row++;
            }
            {
                NumericUpDown nb = GetDefaultNumBox(0, 999999999, 0);
                Label lb = GetDefaultLabel("Score");
                selection_panel.Controls.Add(lb, 0, row);
                selection_panel.Controls.Add(nb, 1, row);
                numBox_score = nb;
                lbs_selection.Add(lb);
                objs_selection.Add(nb);
                row++;
            }
            {
                Label lb = GetDefaultLabel("Color");
                ComboBox combo = GetDefaultCombobox(new string[] { "Red","Blue", "Green" });
                combo.SelectedIndex = 2;

                selection_panel.Controls.Add(lb, 0, row);
                selection_panel.Controls.Add(combo, 1, row);
                comboBox_color = combo;
                lbs_selection.Add(lb);
                objs_selection.Add(combo);
                row++;
            }
            {
                btn_apply = GetDefaultBtn("Apply");
                selection_panel.Controls.Add(btn_apply, 0, row);
                btn_apply.KeyDown+=(o, e) =>
                {
                    if (e.KeyData == Keys.Z)
                    {
                        EnterStage();
                        e.Handled = true;
                    }
                    key_wait_time = 0;
                };
                btn_apply.Click+=(o, e) =>
                {
                    EnterStage();
                };
                row++;
                btn_cancel = GetDefaultBtn("Cancel");
                selection_panel.Controls.Add(btn_cancel, 0, row);
                row++;
                lbs_selection.Add(GetDefaultLabel(""));//empty labels
                objs_selection.Add(btn_apply);
                lbs_selection.Add(GetDefaultLabel(""));
                objs_selection.Add(btn_cancel);
                btn_cancel.KeyDown+=(o, e) =>
                {
                    if (e.KeyData == Keys.Z)
                    {
                        SelectionPanelClose();
                        e.Handled = true;
                    }
                    key_wait_time = 0;
                };
                btn_cancel.Click+=(o, e) =>
                {
                    SelectionPanelClose();
                };
            }

            ComboChange_Stage_Type(comboBox_stage_sel, null);// init this
            Update();
            UpdateSelectedOption();
            PatchRender.actions_render_patch += Update;
        }

        public static void ComboChange_Stage_Type(object sender, EventArgs e)
        {
            if (comboBox_type_sel==null)
                return;
            comboBox_subStage_sel.Items.Clear();

            if(sender == comboBox_stage_sel)
            {
                switch (comboBox_stage_sel.SelectedIndex)
                {
                    case 0://stage 1
                    case 1://2
                    case 2://3
                    case 4://5
                    case 6://7
                        ComboSetItems(comboBox_type_sel, new string[] { "Road", "Boss", "MidBoss" });
                        break;
                    case 3://4
                    case 5://6
                        ComboSetItems(comboBox_type_sel, new string[] { "Road", "Boss" });
                        break;
                }
            }
            SelectedType type = (SelectedType)comboBox_type_sel.SelectedIndex;
            if (type == SelectedType.Boss)//boss
            {
                switch(comboBox_stage_sel.SelectedIndex)
                {
                    default:
                    case 0:
                        ComboSetItems(comboBox_subStage_sel,
                            new string[] { "normal 1", "card 1", "normal 2", "card 2", "FSC" });
                        break;
                    case 1:
                        ComboSetItems(comboBox_subStage_sel,
                            new string[] { "normal 1", "card 1", "normal 2", "card 2", "card 3", "FSC" });
                        break;
                    case 2:
                        ComboSetItems(comboBox_subStage_sel,
                            new string[] { "normal 1", "card 1", "normal 2", "card 2", "normal 3", "card 3" , "FSC" });
                        break;
                    case 3:
                        ComboSetItems(comboBox_subStage_sel,
                            new string[] { "normal 1", "card 1", "normal 2", "card 2", "card 3", "card 4", "card 5",  "FSC" });
                        break;
                    case 4:
                        ComboSetItems(comboBox_subStage_sel, 
                            new string[] { "normal 1", "card 1", "normal 2", "card 2", "normal 3", "card 3", "card 4", "card 5", "FSC" });
                        break;
                    case 5:
                        ComboSetItems(comboBox_subStage_sel, 
                            new string[] { "normal 1", "card 1", "normal 2", "card 2", "normal 3", "card 3", "normal 4", "card 4", "card 5", "card 6", "FSC 1", "FSC 2", "FSC 3" });
                        break;
                    case 6:
                        ComboSetItems(comboBox_subStage_sel,
                            new string[] { "normal 1", "card 1", "normal 2", "card 2", "normal 3", "card 3", "normal 4", "card 4", "normal 5", "card 5", "normal 6", "card 6", "normal 7", "card 7", "normal 8", "card 8", "card 9", "card 10", "FSC" });
                        break;
                }
            }
            else if(type == SelectedType.MidBoss)
            {
                switch (comboBox_stage_sel.SelectedIndex)
                {
                    default:
                    case 0://st1
                    case 1://st2
                    case 4://st5
                        ComboSetItems(comboBox_subStage_sel,
                            new string[] { "normal 1", "card 1"});
                        break;
                    case 2://st3
                        ComboSetItems(comboBox_subStage_sel,
                            new string[] { "card 1", "card 2"});
                        break;
                    case 3://st4
                    case 5://st6
                        ComboSetItems(comboBox_subStage_sel,
                            new string[] { "no midboss..."});
                        break;
                    case 6://st7
                        ComboSetItems(comboBox_subStage_sel,
                            new string[] { "card 1", "card 2" , "card 3" });
                        break;
                }
            }
            else //road
            {
                int stage = comboBox_stage_sel.SelectedIndex;
                if (stage < 0)
                {
                    comboBox_stage_sel.SelectedIndex = 0;
                    stage = 0;
                }
                var idStrs = new string[road_def[stage].Count];
                for(int i = 0; i < idStrs.Length; i++)
                {
                    idStrs[i] = road_def[stage][i].id_name;
                }
                ComboSetItems(comboBox_subStage_sel, idStrs);
            }
            comboBox_subStage_sel.SelectedIndex = 0;
        }

        public static void UpdateSelectedOption()
        {
            for (int i = 0; i < lbs_selection.Count; i++)
            {
                lbs_selection[i].ForeColor = (i==cur_selected_option) ? Color.DarkCyan : Color.Black;
                lbs_selection[i].Font = (i==cur_selected_option) ? PatchMainWind.form_font_bold_u : PatchMainWind.form_font_regular;
                if(i == cur_selected_option){
                    objs_selection[i].Select();
                    objs_selection[i].Focus();
                }
                if (objs_selection[i] is Button)
                {
                    objs_selection[i].ForeColor = (i==cur_selected_option) ? Color.DarkCyan : Color.Black;
                    objs_selection[i].Font = (i==cur_selected_option) ? PatchMainWind.form_font_bold_u : PatchMainWind.form_font_regular;
                }else if (objs_selection[i] is NumericUpDown)
                {
                    NumericUpDown nb = (NumericUpDown)objs_selection[i];
                    nb.Text=$"{nb.Value}";
                }
            }
        }

        public static int key_wait_time = 0;
        public static void Update()
        {
            if (IsSelectionPanelOpen())
            {
                int cnt = menuGroup.StageData.MenuGroupList.Count;
                if (cnt!=0 && menuGroup.StageData.MenuGroupList[cnt-1].GetType() != type_MenuGroup_PlayerSelect)
                {
                    SelectionPanelClose();
                }

                if(key_wait_time > 10)
                {
                    if (PatchInput.key_state.IsPressed(SlimDX.DirectInput.Key.UpArrow))
                    {
                        key_wait_time = 0;
                        cur_selected_option--;
                        if (cur_selected_option == -1)
                            cur_selected_option = lbs_selection.Count - 1;
                        UpdateSelectedOption();
                    }
                    else if (PatchInput.key_state.IsPressed(SlimDX.DirectInput.Key.DownArrow))
                    {
                        key_wait_time = 0;
                        cur_selected_option++;
                        if (cur_selected_option == lbs_selection.Count)
                            cur_selected_option = 0;
                        UpdateSelectedOption();
                    }
                }
            }
            key_wait_time++;
        }

        static public void EnterStage()
        {
            is_Prac = true;
            SelectionPanelClose();

            Point originalPosition = new Point(192, 398);
            switch (menuGroup.MenuItemList[menuGroup.MenuSelectIndex].Name)
            {
                case "FaceReimu_me":
                    menuGroup.StageData.StateSwitchData.SDPswitch.MyPlane = new MyPlane_Reimu(menuGroup.StageData.StateSwitchData.SDPswitch, originalPosition);
                    break;
                case "FaceSanae_me":
                    menuGroup.StageData.StateSwitchData.SDPswitch.MyPlane = new MyPlane_Sanae(menuGroup.StageData.StateSwitchData.SDPswitch, originalPosition);
                    break;
                case "FaceMarisa_me":
                    menuGroup.StageData.StateSwitchData.SDPswitch.MyPlane = new MyPlane_Marisa(menuGroup.StageData.StateSwitchData.SDPswitch, originalPosition);
                    break;
                case "FaceKoishi_me":
                    menuGroup.StageData.StateSwitchData.SDPswitch.MyPlane = new MyPlane_Koishi(menuGroup.StageData.StateSwitchData.SDPswitch, originalPosition);
                    break;
                case "FaceAya_ct":
                    menuGroup.StageData.StateSwitchData.SDPswitch.MyPlane = new MyPlane_Aya(menuGroup.StageData.StateSwitchData.SDPswitch, originalPosition);
                    break;
            }

            InitValues(menuGroup.StageData.StateSwitchData.SDPswitch.MyPlane);

            SelectedType type = (SelectedType)comboBox_type_sel.SelectedIndex;
            int stageIdx = comboBox_stage_sel.SelectedIndex;
            if (stageIdx != 6)//not ex
            {
                switch(type)
                {
                    case SelectedType.Road:
                    case SelectedType.MidBoss:
                        menuGroup.StageData.StateSwitchData.NextState = $"St{stageIdx + 1}";
                        break;
                    case SelectedType.Boss:
                        menuGroup.StageData.StateSwitchData.NextState = $"Bs{stageIdx + 1}";
                        break;
                }
            }else
            {
                menuGroup.StageData.StateSwitchData.SDPswitch.Difficulty = DifficultLevel.Extra;
                menuGroup.StageData.StateSwitchData.NextState = $"StEx";
            }
            menuGroup.StageData.Rep.CreatRpy();
            menuGroup.StageData.StateSwitchData.SDPswitch.SetReplayInfo(menuGroup.StageData.StateSwitchData.NextState);
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
            if (!PracSelection.IsSelectionPanelOpen())
            {
                if (PracSelection.key_wait_time >= 21)// avoid close & open
                    PracSelection.SelectionPanelOpen();
            }
            else {
                PracSelection.EnterStage();
            }
            return false;
        }
    }

    [HarmonyPatch]
    public class PatchPlayerSelectionKeys
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Shooting.MenuGroup_PlayerSelect:ProcessKeys");
        }
        public static bool Prefix(Shooting.BaseMenuGroup __instance)
        {
            if (PracSelection.IsSelectionPanelOpen()) {
                if (__instance.KClass.ArrowLeft && __instance.LastLeft == 0)
                {
                    return false;
                }else if (__instance.KClass.ArrowRight && __instance.LastRight == 0)//disable left/right
                {
                    return false;
                }
                return true;
            }
            return true;
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
            if (!PracSelection.IsSelectionPanelOpen())
            {
                return true;
            }
            if ((__instance.KClass.Key_X || __instance.KClass.Key_ESC) && __instance.LastX == 0)
            {
                PracSelection.SelectionPanelClose();
                __instance.StageData.SoundPlay("se_cancel00.wav");
                return false;
            }
            return true;
        }
    }


    [HarmonyPatch]
    public class PatchMainMenuSelect
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Shooting.MenuGroup_Main:ProcessZ");
        }
        public static void Prefix(Shooting.BaseMenuGroup __instance)
        {
            string name = __instance.MenuItemList[__instance.MenuSelectIndex].Name;
            if(name=="Menu_Start" || name=="Menu_ExtraStart")
            {
                PracSelection.is_Prac = false;
            }
            return;
        }
    }
}
