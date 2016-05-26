using FellSky.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibRocketNet;
using Microsoft.Xna.Framework;
using Artemis;
using FellSky.Systems;
using FellSky.EntityFactories;
using Microsoft.Xna.Framework.Graphics;
using FellSky.Game.Ships;

namespace FellSky.States
{
    public class ShipRefitState: GameState
    {
        public enum EditorMode
        {
            Weapons, Modules, Armor, Build
        }

        public const string RefitScreenGuiDocument = "Gui/RefitScreen.xml";

        private readonly IGuiService GuiService;
        private GraphicsDevice Graphics;

        public EditorMode Mode { get; private set; }

        public ElementDocument Document { get; private set; }
        public static ShipRefitState Instance { get; private set; }
        public EntityWorld World { get; private set; }
        public IReadOnlyList<Ship> Fleet { get; private set; }

        public List<Texture2D> LoadedTextures { get; set; }
        public Entity CurrentShip { get; private set; }

        public ShipRefitState(IReadOnlyList<Ship> fleet)
        {
            if (fleet == null) throw new ArgumentException("Fleet is null.");
            if (fleet.Count < 0) throw new ArgumentException("Fleet is empty.");
            Fleet = fleet;
            GuiService = ServiceLocator.Instance.GetService<IGuiService>();
        }

        public override void LoadContent()
        {
            World = new EntityWorld(false,false,false);
            Instance = this;

            int depth = 0;
            World.SystemManager.SetSystem(new GridRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new BackgroundRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new ShipRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new ParticleSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new BoundingBoxRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new GenericDrawableRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new PhysicsDebugDrawSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            //World.SystemManager.SetSystem(new GuiSystem(), Artemis.Manager.GameLoopType.Draw, depth++);

            int priority = 1;
            World.SystemManager.SetSystem(new CameraControlSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new RigidBodyToTransformSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new MouseControlledTransformSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new BoundingBoxSelectionSystem(), Artemis.Manager.GameLoopType.Update, priority++);

            World.SystemManager.SetSystem(new CoroutineSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            //World.SystemManager.SetSystem(new PlayerShipControlSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new ShipUpdateSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            //World.SystemManager.SetSystem(new StorySystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new PhysicsSystem(), Artemis.Manager.GameLoopType.Update, priority++);

            Graphics = ServiceLocator.Instance.GetService<GraphicsDevice>();
            World.InitializeAll();

            World.CreateCamera(Constants.ActiveCameraTag, Graphics);
            World.CreateGrid(new Vector2(40), new Microsoft.Xna.Framework.Color(30, 30, 60));

            if (Document == null)
            {
                Document = GuiService.Context.LoadDocument(RefitScreenGuiDocument);
                Core.ScriptEvent += (o,e) => Instance?.HandleScriptEvent(o, e);
            }

            //CurrentShip = World.CreateShipEntity(Fleet[0], Vector2.Zero, 0, false);
            Document.Show();
            
            
            base.LoadContent();
        }

        public override void UnloadContent()
        {
            Instance = null;
        }

        public override void Update(GameTime gameTime)
        {
            World.Update();
        }

        public override void Draw(GameTime gameTime)
        {
            Graphics.Clear(Microsoft.Xna.Framework.Color.Black);
            World.Draw();
            GuiService.Context.Render();
        }

        private void SetMode(EditorMode mode)
        {
            if (mode == Mode) return;
            Mode = mode;
            Document.GetElementById("availablePartsList")?.SetProperty("display", "none");
        }

        public void HandleScriptEvent(object sender, ScriptEventArgs args)
        {
            switch (args.Script)
            {
                case "Refit_ChangeMode_Weapons":
                    SetMode(EditorMode.Weapons);
                    break;
                case "Refit_ChangeMode_Armor":
                    SetMode(EditorMode.Armor);
                    break;
                case "Refit_ChangeMode_Modules":
                    SetMode(EditorMode.Modules);
                    break;
                case "Refit_ChangeMode_Build":
                    SetMode(EditorMode.Build);
                    break;
            }
            
        }
    }
}
