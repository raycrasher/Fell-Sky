﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using Microsoft.Xna.Framework;
using FellSky.Systems;
using Microsoft.Xna.Framework.Graphics;
using FellSky.Services;
using FellSky.EntityFactories;
using FellSky.Components;
using FellSky.Game.Ships;

namespace FellSky.States
{
    class MainGameState: GameState
    {
        private GameServiceContainer _services;
        private IGuiService _guiService;
        private Random _rng;

        public EntityWorld World { get; private set; }
        public Entity Camera { get; private set; }

        public MainGameState(GameServiceContainer services)
        {
            _services = services;
            _guiService = _services.GetService<IGuiService>();
        }

        public override void LoadContent()
        {
            CreateWorld();
            
            base.LoadContent();
        }

        private void CreateWorld()
        {
            World = new EntityWorld(false, false, false);
            int depth = 1;
            World.SystemManager.SetSystem(new GridRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new BackgroundRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new ShipRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new ParticleSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new BoundingBoxRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new GenericDrawableRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new PhysicsDebugDrawSystem(), Artemis.Manager.GameLoopType.Draw, depth++);            

            int priority = 1;
            World.SystemManager.SetSystem(new CameraControlSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new RigidBodyToTransformSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new MouseControlledTransformSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new BoundingBoxSelectionSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new GuiSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new CoroutineSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new PlayerShipControlSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new ShipUpdateSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new StorySystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new PhysicsSystem(), Artemis.Manager.GameLoopType.Update, priority++);

            Camera = CameraEntityFactory.CreateCamera(World, Constants.ActiveCameraTag, _services.GetService<GraphicsDevice>());

            World.InitializeAll();

        }

        public override void Update(GameTime gameTime)
        {
            _guiService.Context.Update();
            World.Update();
        }
        public override void Draw(GameTime gameTime)
        {
            GameEngine.Instance.GraphicsDevice.Clear(Color.Black);
            World.Draw();
            _guiService.Context.Render();
        }
    }
}
