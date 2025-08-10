using HarmonyLib;
using Shooting;
using SlimDX.Direct3D9;
using SlimDX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using SlimDX.Windows;
using System.Threading;
using System.Windows.Forms;
using SlimDX.DirectInput;
using System.Collections;
using System.Runtime.Remoting.Contexts;
using SlimDX.XAudio2;

namespace SSS_Prac_Launcher
{
    public class Launcher
    {
        public static Game_Main game_Main;
        static void Main(string[] args)
        {
            Harmony harmony = new Harmony("Hooks");
            harmony.PatchAll();

            // var x = harmony.GetPatchedMethods();
            // foreach (var i in x)
            // {
            //     Console.WriteLine(i.Name);  
            // }
            // Assembly assembly = Assembly.LoadFile(@"C:\disk\touhou\2nd\SSS\SSS\THSSS.exe");

             //Directory.SetCurrentDirectory(@"C:\disk\touhou\2nd\SSS\SSS\");

            {
                bool flag;
                new Mutex(false, "AA", out flag);
                if (flag)
                {
                    System.Windows.Forms.Application.EnableVisualStyles();
                    System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
                    game_Main = new Game_Main();
                    if (game_Main.initSuccess)
                    {
                        MessagePump.Run(game_Main.Form_Main, new MainLoop(game_Main.MainProcess));
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("东方白丝祭(误)已启动，\r\n程序不允许双开。", "程序已启动");
                }
            }
        }
        
    }

    [HarmonyPatch]
    public class PatchMainWind
    {
        public static Panel main_panel;
        public static Form main_form;
        public static System.Drawing.Font form_font_italian;
        public static System.Drawing.Font form_font_regular;
        public static System.Drawing.Font form_font_bold_u;
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Shooting.Game_Main:Direct3DInit");
        }
        public static void Postfix(Shooting.Game_Main __instance)
        {
            var type = AccessTools.GetDeclaredFields(AccessTools.TypeByName("Shooting.RenderForm_Main"));
            FieldInfo f_panel = null;
            foreach (var field in type)
            {
                if (field.Name=="panel1")
                {
                    f_panel = field;
                    break;
                }
            }
            Panel panel = (Panel)f_panel.GetValue(__instance.Form_Main);
            if (panel == null) return;
            main_form = __instance.Form_Main;
            main_panel = panel;

            int fontsize = (int)(main_form.Font.Size*1.5);
            form_font_regular = new System.Drawing.Font(main_form.Font.Name, fontsize, FontStyle.Regular);
            form_font_italian = new System.Drawing.Font(main_form.Font.Name, fontsize, FontStyle.Italic);
            form_font_bold_u = new System.Drawing.Font(main_form.Font.Name, fontsize,  FontStyle.Underline | FontStyle.Bold);

            Prac_Hotkey.Init();
            PracSelection.Init();
        }
    }

    [HarmonyPatch]
    public class PatchRender
    {
        static bool is_inited = false;
        public static MySprite game_sprite;
        public static SlimDX.Direct3D9.Device game_device;
        public static Action actions_render_patch;
        public static void Init(SlimDX.Direct3D9.Device device){
            if (!is_inited)
            {
                game_sprite = new MySprite(device);
                game_device = device;
                is_inited = true;
            }
        }
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Shooting.Game_Main:MainProcess");
        }
        public static SlimDX.Result MyEndScene(SlimDX.Direct3D9.Device device)
        {
            Init(device);
            actions_render_patch();
            return device.EndScene();
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions /*, ILGenerator generator*/)
        {
            var lst_instructions = new List<CodeInstruction>(instructions);
            for (var i = 0; i < lst_instructions.Count; i++)
            {
                if (CodeInstructionExtensions.Calls(lst_instructions[i], SymbolExtensions.GetMethodInfo(() => default(SlimDX.Direct3D9.Device).EndScene())))
                {
                    lst_instructions[i] = CodeInstruction.Call(() => MyEndScene(default));
                    break;
                }
            }
            return lst_instructions.AsEnumerable();
        }
    }



}
