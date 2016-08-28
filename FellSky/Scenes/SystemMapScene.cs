using Artemis;
using FellSky.Systems;
using FellSky.Systems.SceneGraphRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FellSky.Game.Space;
using FellSky.EntityFactories;
using FellSky.Services;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FellSky.Scenes
{
    public class SystemMapScene: Scene
    {
        public SpaceObject CurrentSpaceObject { get; set; }
        public Entity Camera { get; private set; }

        public SystemMapScene(SpaceObject spaceObject)
        {
            CurrentSpaceObject = spaceObject;
        }

        public EntityWorld World { get; private set; }
        public GraphicsDevice Graphics { get; private set; }

        public override void LoadContent()
        {
            int priority = 0, depth = 0;
            World = new EntityWorld(false,false,false);

            Graphics = ServiceLocator.Instance.GetService<GraphicsDevice>();

            World.SystemManager.SetSystem(new GridRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new BackgroundRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            //World.SystemManager.SetSystem(new ShipRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new SceneGraphRendererSystem<StandardShipModelRenderer>(new StandardShipModelRenderer()), Artemis.Manager.GameLoopType.Draw, depth++);
            //World.SystemManager.SetSystem(new BeamRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            //World.SystemManager.SetSystem(new BulletRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new SystemMapSpaceObjectRenderer(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new ParticleSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            //World.SystemManager.SetSystem(new BoundingBoxRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new GenericDrawableRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            //World.SystemManager.SetSystem(new PhysicsDebugDrawSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            //_hardpointSystem = World.SystemManager.SetSystem(new HardpointRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);

            World.SystemManager.SetSystem(new EventSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new CameraControlSystem() { MaxZoom=50 }, Artemis.Manager.GameLoopType.Update, priority++);
            //World.SystemManager.SetSystem(new RigidBodySystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new MouseControlledTransformSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new BoundingBoxSelectionSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new MouseHoverSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new CoroutineSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new PlayerShipControlSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new BeamSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            //World.SystemManager.SetSystem(new BulletSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new ShipUpdateSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            //World.SystemManager.SetSystem(new StorySystem(), Artemis.Manager.GameLoopType.Update, priority++);
            //World.SystemManager.SetSystem(new PhysicsSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            //World.SystemManager.SetSystem(new TurretRotationSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            //World.SystemManager.SetSystem(new WeaponSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new FrameAnimationSystem(), Artemis.Manager.GameLoopType.Update, priority++);

            World.InitializeAll(false);

            Camera = CameraEntityFactory.CreateCamera(World, Constants.ActiveCameraTag, ServiceLocator.Instance.GetService<GraphicsDevice>());
            
            var bgGenerator = ServiceLocator.Instance.GetService<SpaceBackgroundGeneratorService>();
            bgGenerator.GenerateBackground(World, 0.01f, 
                new NebulaParameters(Color.Blue * 0.4f, Vector2.Zero, 4, 1.1f, 4)
                );
            CurrentSpaceObject.CreateEntity(World);
        }

        public override void Update(GameTime gameTime)
        {
            World.Update(gameTime.ElapsedGameTime.Milliseconds);
        }

        public override void Draw(GameTime gameTime)
        {
            Graphics.Clear(Color.Black);
            World.Draw();
        }
    }
}
