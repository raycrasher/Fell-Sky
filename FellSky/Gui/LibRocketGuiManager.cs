using FellSky.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace FellSky.Gui
{
    public static class GuiManager
    {
        private static unsafe LibRocketRenderInterface _renderInterface;
        public static LibRocketNet.Context MainContext { get; private set; }
        public static bool UseVbo = true;
        public static TimeSpan LastRenderTime;

        public static void Initialize(GraphicsDevice graphics, CoroutineManager manager, ContentManager content, KeyboardManager keyboard, MouseManager mouse)
        {
            _renderInterface = new LibRocketRenderInterface(graphics, content, UseVbo);
            LibRocketNet.Core.RenderInterface = _renderInterface;
            LibRocketNet.Core.SystemInterface = new LibRocketSystemInterface();
            LibRocketNet.Core.Initialize();
            MainContext = LibRocketNet.Core.CreateContext("main", new LibRocketNet.Vector2i(graphics.Viewport.Width, graphics.Viewport.Height));
            LibRocketNet.Core.InitDebugger(MainContext);
            LoadFonts();
            // add keyboard hooks

            keyboard.KeyDown += KeyDownHandler;
            keyboard.KeyUp += KeyUpHandler;
            mouse.ButtonDown +=
                (o, e) => MainContext.ProcessMouseButtonDown(e, GetKeyModifiers());
            mouse.ButtonUp +=
                (o, e) => MainContext.ProcessMouseButtonUp(e, GetKeyModifiers());

            mouse.WheelChanged += (w) => MainContext.ProcessMouseWheel(-w, GetKeyModifiers());
            mouse.Move += ProcessMouseMove;

            manager.StartCoroutine(UpdateUI());
        }

        private static void ProcessMouseMove(Microsoft.Xna.Framework.Point p)
        {
            //var pt = _renderInterface.CalcMousePosition(p);
            MainContext.ProcessMouseMove(p.X, p.Y, GetKeyModifiers());
        }

        private static System.Collections.IEnumerable UpdateUI()
        {
            while (true)
            {
                MainContext.Update();
                yield return null;
            }
        }

        private static void KeyDownHandler(Keys k)
        {
            if (k == Keys.F12 && Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl) == true)
            {
                // ui debug mode
                LibRocketNet.Core.DebugMode = !LibRocketNet.Core.DebugMode;
                Console.WriteLine("LibRocket Debug mode {0}.", LibRocketNet.Core.DebugMode ? "enabled" : "disabled");
            }
            MainContext.ProcessKeyDown(TranslateKey(k), GetKeyModifiers());
        }

        private static void LoadFonts()
        {
            if (System.IO.Directory.Exists("Fonts"))
            {
                foreach (var fontfile in System.IO.Directory.GetFiles("Fonts", "*.ttf"))
                {
                    LibRocketNet.Core.LoadFontFace(fontfile);
                    //Console.WriteLine("Loading font \"{0}\"", fontfile);
                }
            }
        }

        private static void KeyUpHandler(Keys k)
        {
            MainContext.ProcessKeyUp(TranslateKey(k), GetKeyModifiers());
        }

        private static LibRocketNet.KeyModifier GetKeyModifiers()
        {
            LibRocketNet.KeyModifier modifiers = 0;
            var kbState = Keyboard.GetState();

            if (kbState.IsKeyDown(Keys.LeftShift) ||
                kbState.IsKeyDown(Keys.RightShift))
                modifiers |= LibRocketNet.KeyModifier.Shift;

            if (kbState.IsKeyDown(Keys.LeftControl) ||
                kbState.IsKeyDown(Keys.RightControl))
                modifiers |= LibRocketNet.KeyModifier.Control;

            if (kbState.IsKeyDown(Keys.LeftAlt) ||
                kbState.IsKeyDown(Keys.RightAlt))
                modifiers |= LibRocketNet.KeyModifier.Alt;

            return modifiers;
        }

        private static LibRocketNet.KeyIdentifiers TranslateKey(Keys key)
        {
            switch (key)
            {
                case Keys.A:
                    return LibRocketNet.KeyIdentifiers.A;

                case Keys.B:
                    return LibRocketNet.KeyIdentifiers.B;

                case Keys.C:
                    return LibRocketNet.KeyIdentifiers.C;

                case Keys.D:
                    return LibRocketNet.KeyIdentifiers.D;

                case Keys.E:
                    return LibRocketNet.KeyIdentifiers.E;

                case Keys.F:
                    return LibRocketNet.KeyIdentifiers.F;

                case Keys.G:
                    return LibRocketNet.KeyIdentifiers.G;

                case Keys.H:
                    return LibRocketNet.KeyIdentifiers.H;

                case Keys.I:
                    return LibRocketNet.KeyIdentifiers.I;

                case Keys.J:
                    return LibRocketNet.KeyIdentifiers.J;

                case Keys.K:
                    return LibRocketNet.KeyIdentifiers.K;

                case Keys.L:
                    return LibRocketNet.KeyIdentifiers.L;

                case Keys.M:
                    return LibRocketNet.KeyIdentifiers.M;

                case Keys.N:
                    return LibRocketNet.KeyIdentifiers.N;

                case Keys.O:
                    return LibRocketNet.KeyIdentifiers.O;

                case Keys.P:
                    return LibRocketNet.KeyIdentifiers.P;

                case Keys.Q:
                    return LibRocketNet.KeyIdentifiers.Q;

                case Keys.R:
                    return LibRocketNet.KeyIdentifiers.R;

                case Keys.S:
                    return LibRocketNet.KeyIdentifiers.S;

                case Keys.T:
                    return LibRocketNet.KeyIdentifiers.T;

                case Keys.U:
                    return LibRocketNet.KeyIdentifiers.U;

                case Keys.V:
                    return LibRocketNet.KeyIdentifiers.V;

                case Keys.W:
                    return LibRocketNet.KeyIdentifiers.W;

                case Keys.X:
                    return LibRocketNet.KeyIdentifiers.X;

                case Keys.Y:
                    return LibRocketNet.KeyIdentifiers.Y;

                case Keys.Z:
                    return LibRocketNet.KeyIdentifiers.Z;

                case Keys.D0:
                    return LibRocketNet.KeyIdentifiers.Num0;

                case Keys.D1:
                    return LibRocketNet.KeyIdentifiers.Num1;

                case Keys.D2:
                    return LibRocketNet.KeyIdentifiers.Num2;

                case Keys.D3:
                    return LibRocketNet.KeyIdentifiers.Num3;

                case Keys.D4:
                    return LibRocketNet.KeyIdentifiers.Num4;

                case Keys.D5:
                    return LibRocketNet.KeyIdentifiers.Num5;

                case Keys.D6:
                    return LibRocketNet.KeyIdentifiers.Num6;

                case Keys.D7:
                    return LibRocketNet.KeyIdentifiers.Num7;

                case Keys.D8:
                    return LibRocketNet.KeyIdentifiers.Num8;

                case Keys.D9:
                    return LibRocketNet.KeyIdentifiers.Num9;

                case Keys.NumPad0:
                    return LibRocketNet.KeyIdentifiers.Numpad0;

                case Keys.NumPad1:
                    return LibRocketNet.KeyIdentifiers.Numpad1;

                case Keys.NumPad2:
                    return LibRocketNet.KeyIdentifiers.Numpad2;

                case Keys.NumPad3:
                    return LibRocketNet.KeyIdentifiers.Numpad3;

                case Keys.NumPad4:
                    return LibRocketNet.KeyIdentifiers.Numpad4;

                case Keys.NumPad5:
                    return LibRocketNet.KeyIdentifiers.Numpad5;

                case Keys.NumPad6:
                    return LibRocketNet.KeyIdentifiers.Numpad6;

                case Keys.NumPad7:
                    return LibRocketNet.KeyIdentifiers.Numpad7;

                case Keys.NumPad8:
                    return LibRocketNet.KeyIdentifiers.Numpad8;

                case Keys.NumPad9:
                    return LibRocketNet.KeyIdentifiers.Numpad9;

                case Keys.Left:
                    return LibRocketNet.KeyIdentifiers.Left;

                case Keys.Right:
                    return LibRocketNet.KeyIdentifiers.Right;

                case Keys.Up:
                    return LibRocketNet.KeyIdentifiers.Up;

                case Keys.Down:
                    return LibRocketNet.KeyIdentifiers.Down;

                case Keys.Add:
                    return LibRocketNet.KeyIdentifiers.Add;

                case Keys.Back:
                    return LibRocketNet.KeyIdentifiers.Back;

                case Keys.Delete:
                    return LibRocketNet.KeyIdentifiers.Delete;

                case Keys.OemQuestion:
                    return LibRocketNet.KeyIdentifiers.Divide;

                case Keys.End:
                    return LibRocketNet.KeyIdentifiers.End;

                case Keys.Escape:
                    return LibRocketNet.KeyIdentifiers.Escape;

                case Keys.F1:
                    return LibRocketNet.KeyIdentifiers.F1;

                case Keys.F2:
                    return LibRocketNet.KeyIdentifiers.F2;

                case Keys.F3:
                    return LibRocketNet.KeyIdentifiers.F3;

                case Keys.F4:
                    return LibRocketNet.KeyIdentifiers.F4;

                case Keys.F5:
                    return LibRocketNet.KeyIdentifiers.F5;

                case Keys.F6:
                    return LibRocketNet.KeyIdentifiers.F6;

                case Keys.F7:
                    return LibRocketNet.KeyIdentifiers.F7;

                case Keys.F8:
                    return LibRocketNet.KeyIdentifiers.F8;

                case Keys.F9:
                    return LibRocketNet.KeyIdentifiers.F9;

                case Keys.F10:
                    return LibRocketNet.KeyIdentifiers.F10;

                case Keys.F11:
                    return LibRocketNet.KeyIdentifiers.F11;

                case Keys.F12:
                    return LibRocketNet.KeyIdentifiers.F12;

                case Keys.F13:
                    return LibRocketNet.KeyIdentifiers.F13;

                case Keys.F14:
                    return LibRocketNet.KeyIdentifiers.F14;

                case Keys.F15:
                    return LibRocketNet.KeyIdentifiers.F15;

                case Keys.Home:
                    return LibRocketNet.KeyIdentifiers.Home;

                case Keys.Insert:
                    return LibRocketNet.KeyIdentifiers.Insert;

                case Keys.LeftControl:
                    return LibRocketNet.KeyIdentifiers.LControl;

                case Keys.LeftShift:
                    return LibRocketNet.KeyIdentifiers.LShift;

                case Keys.Multiply:
                    return LibRocketNet.KeyIdentifiers.Multiply;

                case Keys.Pause:
                    return LibRocketNet.KeyIdentifiers.Pause;

                case Keys.RightControl:
                    return LibRocketNet.KeyIdentifiers.RControl;

                case Keys.Enter:
                    return LibRocketNet.KeyIdentifiers.Return;

                case Keys.RightShift:
                    return LibRocketNet.KeyIdentifiers.RShift;

                case Keys.Space:
                    return LibRocketNet.KeyIdentifiers.Space;

                case Keys.Subtract:
                    return LibRocketNet.KeyIdentifiers.Subtract;

                case Keys.Tab:
                    return LibRocketNet.KeyIdentifiers.Tab;
            }


            return LibRocketNet.KeyIdentifiers.Unknown;
        }

        public static LibRocketNet.ElementDocument LoadDocument(string document)
        {
            var ctx = MainContext.LoadDocument(document);
            return ctx;
        }
    }
}
