using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky
{
    public class Game: Microsoft.Xna.Framework.Game
    {
        public static Game Instance { get; private set; }
        public GraphicsDeviceManager Graphics { get; private set; }
        public Properties.Settings Settings { get { return Properties.Settings.Default; } }

        private Game()
        {
            Instance = this;
            Graphics = new GraphicsDeviceManager(this);
            Graphics.PreferredBackBufferWidth = Settings.GraphicsWidth;
            Graphics.PreferredBackBufferHeight = Settings.GraphicsHeight;
            Graphics.IsFullScreen = Settings.FullScreen;
            Graphics.SynchronizeWithVerticalRetrace = Settings.VSync;
            if (Settings.FrameLimit > 0)
            {
                IsFixedTimeStep = true;
                TargetElapsedTime = TimeSpan.FromSeconds(1.0d / Settings.FrameLimit);
            }
            else
            {
                IsFixedTimeStep = false;
            }
            Graphics.ApplyChanges();
        }

        protected override void Update(GameTime gameTime)
        {
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
