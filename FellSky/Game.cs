using Artemis.System;
using FellSky.Framework;
using FellSky.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

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
        public static GameState State { get; set; }

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
            this.IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            EntitySystem.BlackBoard.SetEntry("GraphicsDevice", GraphicsDevice);
            EntitySystem.BlackBoard.SetEntry("ContentManager", Content);
            Content.RootDirectory = Path.GetFullPath(Settings.DataFolder);
            Environment.CurrentDirectory = Path.GetFullPath(Settings.DataFolder);
            Gui.GuiManager.Initialize(Graphics.GraphicsDevice, Coroutines, Content, Keyboard, Mouse);
            State = new MainGameState();
            State.LoadContent();
            SpriteManager.AddSpriteSheetFromFile(Content,"Textures/hulls.json");
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            Coroutines.RunCoroutines(gameTime.ElapsedGameTime);
            CurrentUpdateTime = gameTime;
            State?.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            State?.Draw(gameTime);
            base.Draw(gameTime);
        }

        static void Main(string[] args)
        {
            new Game();
            Instance.Run();
        }
    }
}
