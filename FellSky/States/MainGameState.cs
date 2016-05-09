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
        public CameraEntityFactory CameraFactory { get; private set; }
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

            CameraFactory = new CameraEntityFactory(World);
            Camera = CameraFactory.CreateCamera(Constants.ActiveCameraTag, _services.GetService<GraphicsDevice>());

            var entity = World.CreateEntity();
            entity.AddComponent(new Transform());

            _rng = new Random();

            var NebulaColors = Enumerable.Range(0, _rng.Next(1,3))
                .Select(r => new Color(_rng.NextFloat(0, 1), _rng.NextFloat(0, 1), _rng.NextFloat(0, 1)))
                .OrderBy(r => new ColorHSL(r).Luminosity)
                .ToArray();

            var spaceGenerator = new SpaceBackgroundGeneratorService(
                GameEngine.Instance.GraphicsDevice, NebulaColors, 
                GameEngine.Instance.GraphicsDevice.Viewport.Width,
                _rng.NextDouble() * 0.18 + 1,
                _rng.NextDouble() * 3 + 2,
                _rng.Next());


            var bgTex = spaceGenerator.GenerateNebula(GameEngine.Instance.GraphicsDevice.Viewport.Width, GameEngine.Instance.GraphicsDevice.Viewport.Height);
            entity.AddComponent(new SpriteComponent { Texture = bgTex, TextureRect = new Rectangle(0, 0, bgTex.Width, bgTex.Height) });
            entity.AddComponent(new BackgroundComponent { Parallax=0, FillViewPort=false });
            entity.Refresh();

            var nebulatex = GameEngine.Instance.Content.Load<Texture2D>($"Textures/nebulae/{_rng.Next(1, 27)}");
            entity = World.CreateEntity();
            entity.AddComponent(new SpriteComponent { Texture = nebulatex, TextureRect = new Rectangle(0, 0, nebulatex.Width, nebulatex.Height) });
            entity.AddComponent(new Transform(Vector2.Zero, 0, new Vector2(0.5f,0.5f)));
            entity.AddComponent(new BackgroundComponent { Parallax = 0, FillViewPort = true, IsAdditive = true });
            entity.Refresh();

            World.InitializeAll();

        }

        public override void Update(GameTime gameTime)
        {
            _guiService.Context.Update();
            World.Update();
        }
        public override void Draw(GameTime gameTime)
        {
            GameEngine.Instance.GraphicsDevice.Clear(Color.CornflowerBlue);
            World.Draw();
            _guiService.Context.Render();
        }
    }
}
