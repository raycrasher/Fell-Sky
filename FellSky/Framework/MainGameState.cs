using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using Microsoft.Xna.Framework;
using FellSky.Graphics;

namespace FellSky.Framework
{
    class MainGameState: GameState
    {
        public EntityWorld World { get; private set; }

        public override void LoadContent()
        {
            World = new EntityWorld(false, true, true);
            Artemis.System.EntitySystem.BlackBoard.SetEntry(Camera2D.PlayerCameraName, new Camera2D());
            base.LoadContent();
        }
        public override void Update(GameTime gameTime)
        {
            World.Update();
            Gui.GuiManager.MainContext.Update();
        }
        public override void Draw(GameTime gameTime)
        {
            Game.Instance.GraphicsDevice.Clear(Color.CornflowerBlue);
            World.Draw();
            Gui.GuiManager.MainContext.Render();
        }
    }
}
