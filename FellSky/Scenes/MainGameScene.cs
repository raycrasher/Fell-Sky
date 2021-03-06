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

using FellSky.Components;
using FellSky.Game.Ships;
using FellSky.Systems.SceneGraphRenderers;
using Microsoft.Xna.Framework.Input;

namespace FellSky.Scenes
{
    class MainGameScene: Scene
    {
        private IGuiService _guiService;

        public Entity Camera { get; private set; }

        public Entity PlayerShip { get; private set; }

        public MainGameScene()
        {
            _guiService = ServiceLocator.Instance.GetService<IGuiService>();
        }

        public override void LoadContent()
        {
            CreateWorld();
            
            base.LoadContent();
        }

        private void CreateWorld()
        {
            int depth = 1;

            World.CreateComponentPool<BulletComponent>();

            World.SystemManager.SetSystem(new GridRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new BackgroundRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new SceneGraphRendererSystem<StandardShipModelRenderer>(new StandardShipModelRenderer()), Artemis.Manager.GameLoopType.Draw, depth++);
            //World.SystemManager.SetSystem(new ShipRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new BeamRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new BulletRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new ParticleSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new DustRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new BoundingBoxRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new GenericDrawableRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new PhysicsDebugDrawSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new GuiSystem(), Artemis.Manager.GameLoopType.Draw, depth++);

            int priority = 1;
            World.SystemManager.SetSystem(new EventSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new CameraControlSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new RigidBodySystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new MouseControlledTransformSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new BoundingBoxSelectionSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new CoroutineSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new PlayerShipControlSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new BeamSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new BulletSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new ShipUpdateSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new StorySystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new PhysicsSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new TurretRotationSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new WeaponSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new FrameAnimationSystem(), Artemis.Manager.GameLoopType.Update, priority++);

            Camera = CameraEntityFactory.CreateCamera(World, Constants.ActiveCameraTag, ServiceLocator.Instance.GetService<GraphicsDevice>());

            World.InitializeAll();
        }

        public override void Update(GameTime gameTime)
        {
            if (KeyboardService.Instance.IsKeyDown(Keys.F1))
            {
                //GameEngine.Instance.ShipRefitScene.CurrentShip = World.TagManager.GetEntity("PlayerShip").GetComponent<ShipComponent>().Ship;
                var refit = new ShipRefitScene(new[] { World.TagManager.GetEntity("PlayerShip")}, World);
                refit.LoadContent();
                refit.PreviousState = this;
                GameEngine.Instance.CurrentScene = refit;
                return;
            }

            World.Update(gameTime.ElapsedGameTime.Milliseconds);
        }
        public override void Draw(GameTime gameTime)
        {
            GameEngine.Instance.GraphicsDevice.Clear(Color.Black);
            World.Draw();
        }

        public override void Enter(Scene previous)
        {
            
        }
    }
}
