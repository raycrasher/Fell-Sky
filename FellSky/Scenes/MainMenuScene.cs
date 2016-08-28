using Artemis;
using FellSky.Components;
using FellSky.EntityFactories;
using FellSky.Services;
using FellSky.Systems;
using LibRocketNet;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace FellSky.Scenes
{
    public class MainMenuScene: Scene
    {
        EntityWorld World;

        public const string MainMenuDocumentPath = "Gui/MainMenu.xml";
        static ElementDocument document;
        private IGuiService _guiService;
        private Entity _bgEntity;

        public static MainMenuScene Instance { get; private set; }

        public override void LoadContent()
        {
            Instance = this;
            World = new EntityWorld(false, false, false);
            int depth = 1;
            World.SystemManager.SetSystem(new GridRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new BackgroundRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            //World.SystemManager.SetSystem(new ShipRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new ParticleSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new BoundingBoxRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new GenericDrawableRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new PhysicsDebugDrawSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            //World.SystemManager.SetSystem(new GuiSystem(), Artemis.Manager.GameLoopType.Draw, depth++);

            int priority = 1;
            World.SystemManager.SetSystem(new CameraControlSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new RigidBodySystem(), Artemis.Manager.GameLoopType.Update, priority++);
            //World.SystemManager.SetSystem(new MouseControlledTransformSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            //World.SystemManager.SetSystem(new BoundingBoxSelectionSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            
            World.SystemManager.SetSystem(new CoroutineSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            //World.SystemManager.SetSystem(new PlayerShipControlSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new ShipUpdateSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            //World.SystemManager.SetSystem(new StorySystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new PhysicsSystem(), Artemis.Manager.GameLoopType.Update, priority++);

            
            _guiService = ServiceLocator.Instance.GetService<IGuiService>();
            InitializeDocument();
            _bgEntity = ServiceLocator.Instance.GetService<SpaceBackgroundGeneratorService>().GenerateBackground(World, GetHashCode());
            World.CreateCamera(Constants.ActiveCameraTag, ServiceLocator.Instance.GetService<GraphicsDevice>());

            World.InitializeAll();
        }

        public override void UnloadContent()
        {
            document.Hide();
            _bgEntity.GetComponent<SpriteComponent>().Texture.Dispose();
        }

        public override void Update(GameTime gameTime)
        {
            World.Update();
        }

        public override void Draw(GameTime gameTime)
        {
            World.Draw();
            _guiService.Context.Render();
        }

        private void InitializeDocument()
        {
            if (document == null)
            {
                LibRocketNet.Core.ScriptEvent += (o, e) => Instance?.OnScriptEvent(o, e);
                document = _guiService.Context.LoadDocument(MainMenuDocumentPath);
            }
            
            document.Show(ElementDocument.FocusFlags.None);
            document.PullToFront();
            
        }

        private void OnScriptEvent(object sender, ScriptEventArgs e)
        {
            switch (e.Script)
            {
                case "MainMenu_Show":
                    document.GetElementById("options")?.SetProperty("display", "none");
                    break;
                case "MainMenu_Continue":
                    document.GetElementById("options")?.SetProperty("display", "none");
                    GameEngine.Instance.CurrentScene = GameEngine.Instance.SystemMapScene;
                    break;
                case "MainMenu_Options":
                    document.GetElementById("options")?.SetProperty("display", "block");
                    break;
                case "MainMenu_Editor":
                    document.GetElementById("options")?.SetProperty("display", "none");
                    break;
                case "MainMenu_Exit":
                    GameEngine.Instance.Exit();
                    break;
            }
        }

        public override void Enter(Scene previous)
        {
            document.Show(ElementDocument.FocusFlags.None);
            document.PullToFront();
            base.Enter(previous);
        }

        public override void Exit(Scene next)
        {
            document.Hide();
            base.Exit(next);
        }
    }
}
