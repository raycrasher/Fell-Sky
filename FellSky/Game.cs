using FellSky.Framework;
using Microsoft.Xna.Framework;
using System;

namespace FellSky
{
    public class Game: Microsoft.Xna.Framework.Game
    {
        public static Game Instance { get; private set; }
        public static GraphicsDeviceManager Graphics { get; private set; }
        public static Properties.Settings Settings { get { return Properties.Settings.Default; } }
        public static GameTime CurrentUpdateTime { get; private set; }
        public static CoroutineManager Coroutines { get; } = new CoroutineManager();
        public static KeyboardManager Keyboard { get; private set; }
        public static MouseManager Mouse { get; private set; }

        private Game()
        {
            Instance = this;
            Graphics = new GraphicsDeviceManager(this);
            Graphics.PreferredBackBufferWidth = Settings.GraphicsWidth;
            Graphics.PreferredBackBufferHeight = Settings.GraphicsHeight;
            Graphics.IsFullScreen = Settings.FullScreen;
            Graphics.SynchronizeWithVerticalRetrace = Settings.VSync;
            IsFixedTimeStep = Settings.FrameLimit > 0;
            if(IsFixedTimeStep) TargetElapsedTime = TimeSpan.FromSeconds(1.0d / Settings.FrameLimit);
            Graphics.ApplyChanges();
            Keyboard = new KeyboardManager(Coroutines);
            Mouse = new MouseManager(Coroutines);
        }

        protected override void Initialize()
        {
            Content.RootDirectory = Settings.DataFolder;
            Gui.GuiManager.Initialize(Graphics.GraphicsDevice, Coroutines, Content, Keyboard, Mouse);
            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            Coroutines.RunCoroutines(gameTime.ElapsedGameTime);
            CurrentUpdateTime = gameTime;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {


            base.Draw(gameTime);
        }

        static void Main(string[] args)
        {
            new Game();
            Instance.Run();
        }
    }
}
