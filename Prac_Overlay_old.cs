//using HarmonyLib;
//using Shooting;
//using SlimDX.Direct3D9;
//using SlimDX.DirectInput;
//using SlimDX;
//using System;
//using System.Collections.Generic;
//using System.Drawing.Imaging;
//using System.Drawing.Text;
//using System.Drawing;
//using System.Linq;
//using System.Reflection;
//using System.Text;

//namespace SSS_Prac_Launcher
//{


//    class Prac_Hotkey
//    {
//        public static List<Prac_Hotkey> hotkeys = new List<Prac_Hotkey>();

//        private static System.Drawing.Font font_R = new System.Drawing.Font("simhei", 12f * GlobalDataPackage.FontScale, FontStyle.Bold);
//        private static System.Drawing.Font font_I = new System.Drawing.Font("simhei", 12f * GlobalDataPackage.FontScale, FontStyle.Italic);
//        private static SlimDX.Direct3D9.Font dxFont_R;
//        private static SlimDX.Direct3D9.Font dxFont_I;
//        private static int y_font = 0;
//        private static int height_font = 0;

//        private static int width = 256;
//        private static int height = 128;

//        public delegate void HotKeyDelegate(bool val);
//        public bool isActivated;
//        private SlimDX.DirectInput.Key hotKey;
//        private string name;
//        private string keyName;
//        HotKeyDelegate hotKeyDelegate;
//        private int wait_time = 0;
//        private Color color;

//        Texture textureR;
//        Texture textureI;
//        public Prac_Hotkey(SlimDX.Direct3D9.Device device, SlimDX.DirectInput.Key keyCode, string keyName, string name, HotKeyDelegate d, Color color)
//        {
//            isActivated = false;
//            hotKey = keyCode;
//            this.name = name;
//            this.keyName = keyName;
//            hotKeyDelegate = d;
//            this.color = color;

//            string render_name = $"[{keyName}] {name}";
//            if (dxFont_R==null)
//            {
//                dxFont_R = new SlimDX.Direct3D9.Font(device, font_R);
//            }
//            if (dxFont_I==null)
//            {
//                dxFont_I = new SlimDX.Direct3D9.Font(device, font_I);
//            }

//            {
//                var bitmapR = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
//                Graphics graphicsR = Graphics.FromImage(bitmapR);
//                graphicsR.TextRenderingHint = TextRenderingHint.AntiAlias;
//                graphicsR.DrawString(render_name, font_R, Brushes.White, 0f, 0f);
//                graphicsR.Dispose();
//                textureR = new Texture(device, width, height, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
//                textureR.GetLevelDescription(0);

//                DataRectangle dataRectangle = textureR.LockRectangle(0, new Rectangle(0, 0, width, height), LockFlags.Discard);
//                BitmapData bitmapData = bitmapR.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, bitmapR.PixelFormat);
//                IntPtr scan = bitmapData.Scan0;
//                dataRectangle.Data.WriteRange(scan, (long)(bitmapData.Width * bitmapData.Height * 4));
//                bitmapR.UnlockBits(bitmapData);
//                textureR.UnlockRectangle(0);
//                bitmapR.Dispose();
//            }

//            {
//                var bitmapI = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
//                Graphics graphicsI = Graphics.FromImage(bitmapI);
//                graphicsI.TextRenderingHint = TextRenderingHint.AntiAlias;
//                graphicsI.DrawString(render_name, font_I, Brushes.White, 0f, 0f);
//                graphicsI.Dispose();
//                textureI = new Texture(device, width, height, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
//                textureI.GetLevelDescription(0);

//                DataRectangle dataRectangle = textureI.LockRectangle(0, new Rectangle(0, 0, width, height), LockFlags.Discard);
//                BitmapData bitmapData = bitmapI.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, bitmapI.PixelFormat);
//                IntPtr scan = bitmapData.Scan0;
//                dataRectangle.Data.WriteRange(scan, (long)(bitmapData.Width * bitmapData.Height * 4));
//                bitmapI.UnlockBits(bitmapData);
//                textureI.UnlockRectangle(0);
//                bitmapI.Dispose();
//            }
//        }

//        public static void BeginGUI(MySprite sprite)
//        {
//            sprite.Begin(SpriteFlags.AlphaBlend);
//            y_font=0;
//        }
//        public static void EndGUI(MySprite sprite)
//        {
//            sprite.End();
//        }
//        public void OnRender(MySprite sprite)
//        {
//            var height = dxFont_R.MeasureString(sprite.sprite, this.name, DrawTextFormat.Left).Height;
//            if (isActivated)
//            {
//                sprite.Draw2D(textureI, new Rectangle(0, 0, width, height), new SizeF(width, height),
//                new PointF(0.0f, y_font), Color.LimeGreen);
//            }
//            else
//            {
//                sprite.Draw2D(textureR, new Rectangle(0, 0, width, height), new SizeF(width, height),
//                new PointF(0.0f, y_font), color);
//            }

//            y_font += height;

//        }
//        public void OnLogic(KeyboardState ks)
//        {
//            if (wait_time>=10 && ks.IsPressed(hotKey))
//            {
//                wait_time=0;
//                isActivated = !isActivated;
//                hotKeyDelegate(isActivated);
//            }
//            wait_time++;
//        }
//        public static void AllGUI(MySprite sprite)
//        {
//            BeginGUI(sprite);
//            foreach (var i in hotkeys)
//            {
//                i.OnRender(sprite);
//            }
//            EndGUI(sprite);
//        }
//        public static void AllLogic(KeyboardState ks)
//        {
//            foreach (var i in hotkeys)
//            {
//                i.OnLogic(ks);
//            }
//        }
//    }


//    [HarmonyPatch]
//    public class OverlayPatches
//    {
//        public static bool is_invincable = false;

//        public static bool is_overlay_opened = false;
//        public static int overlay_open_wait_time = 0;

//        [HarmonyPostfix]
//        [HarmonyPatch(typeof(Shooting.BaseMyPlane), "PreMiss")]
//        public static void BaseMyPlane_PreMiss_Invincable_Prefix(BaseMyPlane __instance)
//        {

//            if (is_invincable)
//                __instance.DeadTime = -1;
//            return;
//        }

//        [HarmonyPostfix]
//        [HarmonyPatch(typeof(Shooting.MyPlane_Koishi), "PreMiss")]
//        public static void MyPlane_Koishi_PreMiss_Invincable_Prefix(BaseMyPlane __instance)
//        {

//            if (is_invincable)
//                __instance.DeadTime = -1;
//            return;
//        }


//        [HarmonyPostfix]
//        [HarmonyPatch(typeof(Shooting.KeyboardCapture), "UpdateInput")]
//        public static void KeyboardCapture_UpdateInput_Postfix(Shooting.KeyboardCapture __instance)
//        {
//            if (__instance==null || __instance.currentKeyboardState==null)
//                return;
//            if (is_overlay_opened)
//                Prac_Hotkey.AllLogic(__instance.currentKeyboardState);
//            if (overlay_open_wait_time > 10)
//            {
//                if (__instance.currentKeyboardState.IsPressed(Key.Backspace))
//                {
//                    is_overlay_opened = !is_overlay_opened;
//                    overlay_open_wait_time=0;
//                }
//            }
//            overlay_open_wait_time++;
//            return;
//        }
//    }

//    [HarmonyPatch]
//    class OverLayRenderPatch
//    {
//        static bool is_inited = false;
//        static MySprite sprite;
//        public static void Init(SlimDX.Direct3D9.Device device)
//        {
//            if (!is_inited)
//            {
//                Prac_Hotkey.hotkeys.Add(new Prac_Hotkey(device, SlimDX.DirectInput.Key.F1, "F1", "invincable", (bool s) => { OverlayPatches.is_invincable = s; }, Color.White));
//                Prac_Hotkey.hotkeys.Add(new Prac_Hotkey(device, SlimDX.DirectInput.Key.F2, "F2", "test", (bool s) => { }, Color.White));
//                sprite = new MySprite(device);
//                is_inited = true;
//            }
//        }
//        public static MethodBase TargetMethod()
//        {
//            return AccessTools.Method("Shooting.Game_Main:MainProcess");
//        }
//        public static SlimDX.Result MyEndScene(SlimDX.Direct3D9.Device device)
//        {
//            Init(device);
//            if (OverlayPatches.is_overlay_opened)
//                Prac_Hotkey.AllGUI(sprite);

//            return device.EndScene();
//            // ...
//        }

//        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions /*, ILGenerator generator*/)
//        {
//            var lst_instructions = new List<CodeInstruction>(instructions);
//            for (var i = 0; i < lst_instructions.Count; i++)
//            {
//                if (CodeInstructionExtensions.Calls(lst_instructions[i], SymbolExtensions.GetMethodInfo(() => default(SlimDX.Direct3D9.Device).EndScene())))
//                {
//                    lst_instructions[i] = CodeInstruction.Call(() => MyEndScene(default));
//                    break;
//                }
//            }
//            return lst_instructions.AsEnumerable();

//            //var codeMatcher = new CodeMatcher(instructions);
//            //   codeMatcher.MatchStartForward(
//            //       CodeMatch.Calls(() => default(SlimDX.Direct3D9.Device).EndScene())
//            //   )
//            //   .ThrowIfInvalid("Could not find call")
//            //   .RemoveInstruction()
//            //   .InsertAndAdvance(
//            //       CodeInstruction.Call(() => MyEndScene(default))
//            //   );

//        }
//    }
//}
