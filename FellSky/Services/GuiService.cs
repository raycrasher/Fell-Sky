using Microsoft.Xna.Framework.Graphics;
using System;
using LibRocketNet;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace FellSky.Services
{
    public interface IGuiService
    {
        Context Context { get; }
    }

    class GuiService : IGuiService
    {
        public const string ContextName = "MainContext";

        private GraphicsDevice _graphics;
        private IKeyboardService _keyboard;
        private IMouseService _mouse;
        private ITimerService _timer;
        private RenderInterface _renderInterface;
        private ICoroutineService _coroutines;

        public Context Context { get; private set; }

        public GuiService(GraphicsDevice graphics, ITimerService timer, IKeyboardService keyboard, IMouseService mouse, ContentManager content, ICoroutineService coroutines)
        {
            _graphics = graphics;
            _timer = timer;
            _keyboard = keyboard;
            _mouse = mouse;
            _renderInterface = new Gui.LibRocketRenderInterface(_graphics, content);
            _coroutines = coroutines;

            LibRocketNet.Core.SystemInterface = new Gui.LibRocketSystemInterface(_timer);
            LibRocketNet.Core.Initialize();

            Context = LibRocketNet.Core.CreateContext(
                ContextName,
                new Vector2i(graphics.Viewport.Width, graphics.Viewport.Height),
                _renderInterface);

            LoadFonts();
            LibRocketNet.Core.InitDebugger(Context);

            keyboard.KeyDown += KeyDownHandler;
            keyboard.KeyUp += KeyUpHandler;
            mouse.ButtonDown +=
                (o, e) => Context.ProcessMouseButtonDown(e, GetKeyModifiers());
            mouse.ButtonUp +=
                (o, e) => Context.ProcessMouseButtonUp(e, GetKeyModifiers());

            mouse.WheelChanged += (w) => Context.ProcessMouseWheel(-w, GetKeyModifiers());
            mouse.Move += ProcessMouseMove;

            coroutines.StartCoroutine(UpdateUI());
        }

        private void LoadFonts()
        {
            if (System.IO.Directory.Exists("Fonts"))
            {
                foreach (var fontfile in System.IO.Directory.GetFiles("Fonts", "*.ttf"))
                {
                    LibRocketNet.Core.LoadFontFace(fontfile);
                }
            }
        }

        private void ProcessMouseMove(Microsoft.Xna.Framework.Point p)
        {
            //var pt = _renderInterface.CalcMousePosition(p);
            Context.ProcessMouseMove(p.X, p.Y, GetKeyModifiers());
        }

        private System.Collections.IEnumerable UpdateUI()
        {
            while (true)
            {
                Context.Update();
                yield return null;
            }
        }

        private void KeyDownHandler(Keys k)
        {
            if (k == Keys.F12 && Keyboard.GetState().IsKeyDown(Keys.LeftControl) == true)
            {
                // ui debug mode
                LibRocketNet.Core.DebugMode = !LibRocketNet.Core.DebugMode;
                Console.WriteLine("LibRocket Debug mode {0}.", LibRocketNet.Core.DebugMode ? "enabled" : "disabled");
            }
            Context.ProcessKeyDown(TranslateKey(k), GetKeyModifiers());
        }

        private void KeyUpHandler(Keys k)
        {
            Context.ProcessKeyUp(TranslateKey(k), GetKeyModifiers());
        }

        private KeyModifier GetKeyModifiers()
        {
            KeyModifier modifiers = 0;
            var kbState = Keyboard.GetState();

            if (kbState.IsKeyDown(Keys.LeftShift) ||
                kbState.IsKeyDown(Keys.RightShift))
                modifiers |= KeyModifier.Shift;

            if (kbState.IsKeyDown(Keys.LeftControl) ||
                kbState.IsKeyDown(Keys.RightControl))
                modifiers |= KeyModifier.Control;

            if (kbState.IsKeyDown(Keys.LeftAlt) ||
                kbState.IsKeyDown(Keys.RightAlt))
                modifiers |= KeyModifier.Alt;

            return modifiers;
        }

        private static KeyIdentifiers TranslateKey(Keys key)
        {
            switch (key)
            {
                case Keys.A:
                    return KeyIdentifiers.A;

                case Keys.B:
                    return KeyIdentifiers.B;

                case Keys.C:
                    return KeyIdentifiers.C;

                case Keys.D:
                    return KeyIdentifiers.D;

                case Keys.E:
                    return KeyIdentifiers.E;

                case Keys.F:
                    return KeyIdentifiers.F;

                case Keys.G:
                    return KeyIdentifiers.G;

                case Keys.H:
                    return KeyIdentifiers.H;

                case Keys.I:
                    return KeyIdentifiers.I;

                case Keys.J:
                    return KeyIdentifiers.J;

                case Keys.K:
                    return KeyIdentifiers.K;

                case Keys.L:
                    return KeyIdentifiers.L;

                case Keys.M:
                    return KeyIdentifiers.M;

                case Keys.N:
                    return KeyIdentifiers.N;

                case Keys.O:
                    return KeyIdentifiers.O;

                case Keys.P:
                    return KeyIdentifiers.P;

                case Keys.Q:
                    return KeyIdentifiers.Q;

                case Keys.R:
                    return KeyIdentifiers.R;

                case Keys.S:
                    return KeyIdentifiers.S;

                case Keys.T:
                    return KeyIdentifiers.T;

                case Keys.U:
                    return KeyIdentifiers.U;

                case Keys.V:
                    return KeyIdentifiers.V;

                case Keys.W:
                    return KeyIdentifiers.W;

                case Keys.X:
                    return KeyIdentifiers.X;

                case Keys.Y:
                    return KeyIdentifiers.Y;

                case Keys.Z:
                    return KeyIdentifiers.Z;

                case Keys.D0:
                    return KeyIdentifiers.Num0;

                case Keys.D1:
                    return KeyIdentifiers.Num1;

                case Keys.D2:
                    return KeyIdentifiers.Num2;

                case Keys.D3:
                    return KeyIdentifiers.Num3;

                case Keys.D4:
                    return KeyIdentifiers.Num4;

                case Keys.D5:
                    return KeyIdentifiers.Num5;

                case Keys.D6:
                    return KeyIdentifiers.Num6;

                case Keys.D7:
                    return KeyIdentifiers.Num7;

                case Keys.D8:
                    return KeyIdentifiers.Num8;

                case Keys.D9:
                    return KeyIdentifiers.Num9;

                case Keys.NumPad0:
                    return KeyIdentifiers.Numpad0;

                case Keys.NumPad1:
                    return KeyIdentifiers.Numpad1;

                case Keys.NumPad2:
                    return KeyIdentifiers.Numpad2;

                case Keys.NumPad3:
                    return KeyIdentifiers.Numpad3;

                case Keys.NumPad4:
                    return KeyIdentifiers.Numpad4;

                case Keys.NumPad5:
                    return KeyIdentifiers.Numpad5;

                case Keys.NumPad6:
                    return KeyIdentifiers.Numpad6;

                case Keys.NumPad7:
                    return KeyIdentifiers.Numpad7;

                case Keys.NumPad8:
                    return KeyIdentifiers.Numpad8;

                case Keys.NumPad9:
                    return KeyIdentifiers.Numpad9;

                case Keys.Left:
                    return KeyIdentifiers.Left;

                case Keys.Right:
                    return KeyIdentifiers.Right;

                case Keys.Up:
                    return KeyIdentifiers.Up;

                case Keys.Down:
                    return KeyIdentifiers.Down;

                case Keys.Add:
                    return KeyIdentifiers.Add;

                case Keys.Back:
                    return KeyIdentifiers.Back;

                case Keys.Delete:
                    return KeyIdentifiers.Delete;

                case Keys.OemQuestion:
                    return KeyIdentifiers.Divide;

                case Keys.End:
                    return KeyIdentifiers.End;

                case Keys.Escape:
                    return KeyIdentifiers.Escape;

                case Keys.F1:
                    return KeyIdentifiers.F1;

                case Keys.F2:
                    return KeyIdentifiers.F2;

                case Keys.F3:
                    return KeyIdentifiers.F3;

                case Keys.F4:
                    return KeyIdentifiers.F4;

                case Keys.F5:
                    return KeyIdentifiers.F5;

                case Keys.F6:
                    return KeyIdentifiers.F6;

                case Keys.F7:
                    return KeyIdentifiers.F7;

                case Keys.F8:
                    return KeyIdentifiers.F8;

                case Keys.F9:
                    return KeyIdentifiers.F9;

                case Keys.F10:
                    return KeyIdentifiers.F10;

                case Keys.F11:
                    return KeyIdentifiers.F11;

                case Keys.F12:
                    return KeyIdentifiers.F12;

                case Keys.F13:
                    return KeyIdentifiers.F13;

                case Keys.F14:
                    return KeyIdentifiers.F14;

                case Keys.F15:
                    return KeyIdentifiers.F15;

                case Keys.Home:
                    return KeyIdentifiers.Home;

                case Keys.Insert:
                    return KeyIdentifiers.Insert;

                case Keys.LeftControl:
                    return KeyIdentifiers.LControl;

                case Keys.LeftShift:
                    return KeyIdentifiers.LShift;

                case Keys.Multiply:
                    return KeyIdentifiers.Multiply;

                case Keys.Pause:
                    return KeyIdentifiers.Pause;

                case Keys.RightControl:
                    return KeyIdentifiers.RControl;

                case Keys.Enter:
                    return KeyIdentifiers.Return;

                case Keys.RightShift:
                    return KeyIdentifiers.RShift;

                case Keys.Space:
                    return KeyIdentifiers.Space;

                case Keys.Subtract:
                    return KeyIdentifiers.Subtract;

                case Keys.Tab:
                    return KeyIdentifiers.Tab;
            }


            return KeyIdentifiers.Unknown;
        }

        public ElementDocument LoadDocument(string document)
        {
            var doc = Context.LoadDocument(document);
            return doc;
        }
    }
}
