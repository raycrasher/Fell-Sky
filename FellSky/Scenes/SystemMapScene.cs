using Artemis;
using FellSky.Systems;
using FellSky.Systems.SceneGraphRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FellSky.Game.Space;

using FellSky.Services;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Artemis.Interface;

namespace FellSky.Scenes
{
    public class SystemMapScene: Scene
    {
        private Texture2D _bgTexture;

        public SpaceObject CurrentSpaceObject { get; set; }
        public Entity Camera { get; private set; }

        public LibRocketNet.ElementDocument UiDocument { get; private set; }

        public SystemMapScene(SpaceObject spaceObject)
        {
            CurrentSpaceObject = spaceObject;
        }

        public GraphicsDevice Graphics { get; private set; }

        public override void LoadContent()
        {
            int priority = 0, depth = 0;

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

            World.InitializeAll();

            Camera = CameraEntityFactory.CreateCamera(World, Constants.ActiveCameraTag, ServiceLocator.Instance.GetService<GraphicsDevice>());
            
            var bgGenerator = ServiceLocator.Instance.GetService<SpaceBackgroundGeneratorService>();
            var bgEntity = bgGenerator.GenerateBackground(World, 0.01f, 
                new NebulaParameters(Color.Blue * 0.4f, Vector2.Zero, 4, 1.1f, 4)
                );

            _bgTexture = bgEntity.GetComponent<Components.SpriteComponent>().Texture;

            CurrentSpaceObject.CreateEntity(World);

            UiDocument = ServiceLocator.Instance.GetService<IGuiService>().Context.LoadDocument("Gui/SystemMap.xml");
        }

        public override void UnloadContent()
        {
            _bgTexture?.Dispose();
            _bgTexture = null;
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

        public override void Enter(Scene previous)
        {
            UiDocument.Show(LibRocketNet.ElementDocument.FocusFlags.Focus);
        }

        public override void Exit(Scene next)
        {
            UiDocument.Hide();            
        }
    }
}
