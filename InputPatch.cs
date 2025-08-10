using HarmonyLib;
using Shooting;
using SlimDX;
using SlimDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace SSS_Prac_Launcher
{
    [HarmonyPatch]
    class PatchInputINI
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Shooting.INI_RW:IniReadValue");
        }
        public static bool Prefix(Shooting.INI_RW __instance, string Section, string Key,ref string __result)
        {
            if (Key == "UseDirectInput")
            {
                __result = "1";
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch]
    class PatchInput
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Shooting.KeyboardCapture:UpdateInput");
        }
        public static Action actions_input_patch;
        public static Keyboard keyboard = null;
        public static KeyboardState key_state;
        public static bool Prefix(Shooting.KeyboardCapture __instance, ref KeyClass KClass)
        {
            if(keyboard == null)
            {
                FieldInfo f_kb = null;
                var type = AccessTools.GetDeclaredFields(AccessTools.TypeByName("Shooting.KeyboardCapture"));
                foreach (var f in type)
                {
                    if (f.Name == "keyboard")
                    {
                        f_kb = f;
                        break;
                    }
                }
                keyboard = (Keyboard)f_kb.GetValue(__instance);
            }
            
            if (keyboard.Acquire().IsFailure)
            {
                return false;
            }
            if (keyboard.Poll().IsFailure)
            {
                return false;
            }
            __instance.lastKeyboardState = __instance.currentKeyboardState;
            __instance.currentKeyboardState = keyboard.GetCurrentState();
            if (Result.Last.IsFailure)
            {
                return false;
            }
            KClass.ArrowLeft = __instance.currentKeyboardState.IsPressed(Key.LeftArrow);
            KClass.ArrowRight = __instance.currentKeyboardState.IsPressed(Key.RightArrow);
            KClass.ArrowUp = __instance.currentKeyboardState.IsPressed(Key.UpArrow);
            KClass.ArrowDown = __instance.currentKeyboardState.IsPressed(Key.DownArrow);
            KClass.Key_Shift = __instance.currentKeyboardState.IsPressed(Key.LeftShift);
            KClass.Key_Z = (__instance.currentKeyboardState.IsPressed(Key.Z) | __instance.currentKeyboardState.IsPressed(Key.Return));
            KClass.Key_X = __instance.currentKeyboardState.IsPressed(Key.X);
            KClass.Key_C = __instance.currentKeyboardState.IsPressed(Key.R);
            KClass.Key_Ctrl = __instance.currentKeyboardState.IsPressed(Key.LeftControl);
            KClass.Key_ESC = __instance.currentKeyboardState.IsPressed(Key.Escape);
            KClass.Key_plus = __instance.currentKeyboardState.IsPressed(Key.Equals);
            KClass.Key_minus = __instance.currentKeyboardState.IsPressed(Key.Minus);
            KClass.Key_plus |= KClass.Key_Ctrl;
            KClass.Key_minus |= KClass.Key_Shift;

            key_state = keyboard.GetCurrentState();
            actions_input_patch();
            return false;
        }
    }

    [HarmonyPatch]
    class PatchInputInit
    {
        public static MethodBase TargetMethod()
        {
            return (MethodBase)(AccessTools.TypeByName("Shooting.KeyboardCapture").GetMember(".ctor", AccessTools.all)[0]);
        }
        public static bool Prefix(Shooting.KeyboardCapture __instance, IntPtr handle)
        {
            DirectInput directInput = new DirectInput();
            //CooperativeLevel flags = CooperativeLevel.NoWinKey | CooperativeLevel.Foreground | CooperativeLevel.Exclusive;
            CooperativeLevel flags = CooperativeLevel.NoWinKey | CooperativeLevel.Foreground | CooperativeLevel.Nonexclusive;
            Keyboard keyboard;
            try
            {
        
                keyboard = new Keyboard(directInput);
                keyboard.SetCooperativeLevel(handle, flags);
            }
            catch (DirectInputException ex)
            {
                MessageBox.Show("DirectInput Error " + ex.Message);
                return false;
            }
            keyboard.Acquire();
        
            var type = AccessTools.GetDeclaredFields(AccessTools.TypeByName("Shooting.KeyboardCapture"));
            FieldInfo f_kb = null;
            foreach (var f in type)
            {
                if (f.Name == "keyboard")
                {
                    f_kb = f;
                    break;
                }
            }
            if (f_kb != null)
            {
                f_kb.SetValue(__instance, keyboard);
            }
            return false;
        }
    }
}
