using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using Microsoft.Xna.Framework;
using FellSky.Systems;
using Microsoft.Xna.Framework.Graphics;
using FellSky.Services;

namespace FellSky.States
{
    class MainGameState: GameState
    {
        const string CameraTag = "Camera";
        private GameServiceContainer _services;

        public EntityWorld World { get; private set; }

        public MainGameState(GameServiceContainer services)
        {
            _services = services;
        }

        public override void LoadContent()
        {
            CreateWorld();
            
            base.LoadContent();
        }

        private void CreateWorld()
        {
            World = new EntityWorld(false, false, false);
            World.SystemManager.SetSystem(new ShipRendererSystem(_services.GetService<SpriteBatch>(), CameraTag), Artemis.Manager.GameLoopType.Draw, 1);
            World.InitializeAll();
        }

        public override void Update(GameTime gameTime)
        {
            _services.GetService<IGuiService>().Context.Update();
            World.Update();
            
        }
        public override void Draw(GameTime gameTime)
        {
            Game.Instance.GraphicsDevice.Clear(Color.CornflowerBlue);
            World.Draw();
            _services.GetService<IGuiService>().Context.Render();
        }
    }
}
