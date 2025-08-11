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
        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool AllocConsole();

        [System.Runtime.InteropServices.DllImport("Kernel32")]
        public static extern void FreeConsole();

        public static Game_Main game_Main;
        static void Main(string[] args)
        {
            bool show_cmd = false;
            for (int i = 0; i<args.Length; i++)
            {
                if (args[i]=="-c" || args[i]=="-C")
                    show_cmd = true;
            }
            if (show_cmd)
                AllocConsole();
            try
            {
                Harmony harmony = new Harmony("Hooks");
                harmony.PatchAll();
                var x = harmony.GetPatchedMethods();
                foreach (var i in x)
                {
                    Console.WriteLine($"Patched: {i.ReflectedType.FullName}:{i.Name}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Patch Error:{ex.StackTrace}\n{ex.Message}");
            }
            try
            {
                // Directory.SetCurrentDirectory(@"C:\disk\touhou\2nd\SSS\SSS\");
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
            catch (Exception ex)
            {
                MessageBox.Show($"Launch Error:{ex.StackTrace}\n{ex.Message}");
            }
        }

    }

    [HarmonyPatch]
    public class PatchMainWind
    {
        public static Panel overlay_panel;
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
            // Shooting.Game_Main:Direct3DInit is used after setting the window size
            var windSz = GetReflection.GetField("Shooting.Game_Main", "windowSize");
            int windsz = (int)windSz.GetValue(__instance);
            if(windsz >=4 )
            {
                if (windsz == 4)
                {
                    __instance.Form_Main.ClientSize = new Size(1440, 1080);
                }
                else if (windsz == 5)
                {
                    __instance.Form_Main.ClientSize = new Size(1920, 1280);
                }
                else if (windsz == 6)
                {
                    __instance.Form_Main.ClientSize = new Size(2560, 1920);
                }
                else if (windsz == 7)
                {
                    __instance.Form_Main.ClientSize = new Size(2880, 2160);
                }
                __instance.Form_Main.Refresh();
            }
            

            FieldInfo f_panel = GetReflection.GetField("Shooting.RenderForm_Main", "panel1");
            Panel panel = (Panel)f_panel.GetValue(__instance.Form_Main);
            main_form = __instance.Form_Main;
            overlay_panel = panel;


            float sz = main_form.Width/960.0f*1.5f;
            int fontsize = (int)(main_form.Font.Size*sz);
            form_font_regular = new System.Drawing.Font(main_form.Font.Name, fontsize, FontStyle.Regular);
            form_font_italian = new System.Drawing.Font(main_form.Font.Name, fontsize, FontStyle.Italic);
            form_font_bold_u = new System.Drawing.Font(main_form.Font.Name, fontsize, FontStyle.Underline | FontStyle.Bold);
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
        public static void Init(SlimDX.Direct3D9.Device device)
        {
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
            actions_render_patch?.Invoke();
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

    public class GetReflection
    {
        public static FieldInfo GetField(string typename, string name)
        {
            var type = AccessTools.GetDeclaredFields(AccessTools.TypeByName(typename));
            foreach (var field in type)
            {
                if (field.Name == name)
                {
                    return field;
                }
            }
            return null;
        }

        public static MethodInfo GetMethod(string typename, string name)
        {
            var type = AccessTools.GetDeclaredMethods(AccessTools.TypeByName(typename));
            foreach (var method in type)
            {
                if (method.Name == name)
                {
                    return method;
                }
            }
            return null;
        }

        public static PropertyInfo GetProperty(string typename, string name)
        {
            var type = AccessTools.GetDeclaredProperties(AccessTools.TypeByName(typename));
            foreach (var prop in type)
            {
                if (prop.Name == name)
                {
                    return prop;
                }
            }
            return null;
        }
    }

    [HarmonyPatch]
    public class PatchWindowSize
    {

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Shooting.Game_Main:WindowSwitch");
        }

        public static bool Prefix(Game_Main __instance)
        {
            var full_wind = GetReflection.GetField("Shooting.Game_Main", "fullWindow");
            var windSz = GetReflection.GetField("Shooting.Game_Main", "windowSize");
            var ToggleUnrealFullScreen = GetReflection.GetMethod("Shooting.Game_Main", "ToggleUnrealFullScreen");
            var ToggleFullScreen = GetReflection.GetMethod("Shooting.Game_Main", "ToggleFullScreen");
            if ((bool)full_wind.GetValue(__instance))
            {
                if (Game_Main.useUnrealFullScreen)
                {
                    ToggleUnrealFullScreen.Invoke(__instance, null);
                }
                else
                {
                    ToggleFullScreen.Invoke(__instance, null);
                    __instance.Form_Main.ClientSize = new Size(640, 480);
                }
            }
            else
            {
                int sz = (int)windSz.GetValue(__instance);
                switch (sz)
                {
                    case 0:
                        sz++;
                        __instance.Form_Main.ClientSize = new Size(800, 600);
                        break;
                    case 1:
                        sz++;
                        __instance.Form_Main.ClientSize = new Size(1024, 768);
                        break;
                    case 2:
                        sz++;
                        __instance.Form_Main.ClientSize = new Size(1280, 960);
                        break;
                    case 3:
                        sz++;
                        __instance.Form_Main.ClientSize = new Size(1440, 1080);
                        break;
                    case 4:
                        sz++;
                        __instance.Form_Main.ClientSize = new Size(1920, 1280);
                        break;
                    case 5:
                        sz++;
                        __instance.Form_Main.ClientSize = new Size(2560, 1920);
                        break;
                    case 6:
                        sz++;
                        __instance.Form_Main.ClientSize = new Size(2880, 2160);
                        break;
                    case 7:
                        // disable toggle fullscreen
                        // sz = 0;
                        // if (Game_Main.useUnrealFullScreen)
                        // {
                        //     ToggleUnrealFullScreen.Invoke(__instance, null);
                        // }
                        // else
                        // {
                        //     ToggleFullScreen.Invoke(__instance, null);
                        // }
                        // break;
                    default:
                        sz = 0;
                        __instance.Form_Main.ClientSize = new Size(640, 480);
                        break;
                }
                windSz.SetValue(__instance, sz);
            }
            __instance.Form_Main.Refresh();
            return false;
        }
    }
}
