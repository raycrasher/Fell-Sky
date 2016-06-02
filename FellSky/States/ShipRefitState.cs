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
using FellSky.Components;

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
        private HardpointRendererSystem _hardpointSystem;
        private IMouseService _mouse;
        private Point _lastClickPos;

        public EditorMode Mode { get; private set; }

        public ElementDocument Document { get; private set; }
        public static ShipRefitState Instance { get; private set; }
        public EntityWorld World { get; private set; }
        public IReadOnlyList<Ship> Fleet { get; private set; }

        public List<Texture2D> LoadedTextures { get; set; }
        public Entity CurrentShip { get; private set; }
        public Entity GridEntity { get; private set; }
        public Entity CameraEntity { get; private set; }

        public ShipRefitState(IReadOnlyList<Ship> fleet)
        {
            if (fleet == null) throw new ArgumentException("Fleet is null.");
            if (fleet.Count < 0) throw new ArgumentException("Fleet is empty.");
            Fleet = fleet;
            GuiService = ServiceLocator.Instance.GetService<IGuiService>();
            _mouse = ServiceLocator.Instance.GetService<IMouseService>();
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
            _hardpointSystem = World.SystemManager.SetSystem(new HardpointRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            //World.SystemManager.SetSystem(new GuiSystem(), Artemis.Manager.GameLoopType.Draw, depth++);

            int priority = 1;
            World.SystemManager.SetSystem(new CameraControlSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new RigidBodyToTransformSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new MouseControlledTransformSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new BoundingBoxSelectionSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new MouseHoverSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new CoroutineSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            //World.SystemManager.SetSystem(new PlayerShipControlSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new ShipUpdateSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            //World.SystemManager.SetSystem(new StorySystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new PhysicsSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            

            Graphics = ServiceLocator.Instance.GetService<GraphicsDevice>();
            World.InitializeAll();

            CameraEntity = World.CreateCamera(Constants.ActiveCameraTag, Graphics);
            GridEntity = World.CreateGrid(new Vector2(100), new Microsoft.Xna.Framework.Color(30, 30, 60));

            if (Document == null)
            {
                Document = GuiService.Context.LoadDocument(RefitScreenGuiDocument);
                Core.ScriptEvent += (o,e) => Instance?.HandleScriptEvent(o, e);
            }

            

            CurrentShip = World.CreateShipEntity(Fleet[0], Vector2.Zero, 0, false);
            AddHardpointsToShip(CurrentShip);

            SetMode(EditorMode.Weapons);

            Document.Show();

            _mouse.ButtonDown += HandleMouseButtonDown;
            _mouse.ButtonUp += HandleMouseButtonUp;

            Document.GetElementById("availablePartsList").SetProperty("display", "none");

            base.LoadContent();
        }

        private void HandleMouseButtonDown(Point pos, int button)
        {
            _lastClickPos = pos;

            if (Mode == EditorMode.Weapons && button == 0 && _hoverEntities.Count > 0)
                ShowPartList(_hoverEntities.First());
            
        }

        private void ShowPartList(PartEntityPair partEntityPair)
        {
            var hardpoint = partEntityPair.Entity.GetComponent<HardpointComponent>();
            Document.GetElementById("availablePartsList").SetProperty("display", "block");
        }

        private void HandleMouseButtonUp(Point pos, int button)
        {
            if(button == 1)
            {
                var delta = pos - _lastClickPos;
                if (new Vector2(delta.X, delta.Y).LengthSquared() < 2)
                {
                    var control = World.SystemManager.GetSystem<CameraControlSystem>();
                    control.MoveTo(Vector2.Zero);
                    control.ZoomTo(1);
                }
                    

            }
        }

        private readonly HashSet<PartEntityPair> _hoverEntities = new HashSet<PartEntityPair>();

        private void AddHardpointsToShip(Entity ship)
        {
            var shipComponent = ship.GetComponent<ShipComponent>();
            const float BaseAlpha = 0.3f;
            foreach (var partEntity in shipComponent.PartEntities.Where(s=>shipComponent.Ship.Hardpoints.Any(h=>h.Hull == s.Part))){
                var hardpoint = new HardpointArcDrawingComponent { DrawHardpointIcon = true, Alpha = BaseAlpha };
                partEntity.Entity.AddComponent(hardpoint);
                var entity = World.CreateEntity();
                var xform = partEntity.Part.Transform;
                var box = HardpointRendererSystem.GetIconBoundingBox(partEntity.Entity.GetComponent<HardpointComponent>().Hardpoint);
                entity.AddComponent(new Transform(xform.Position - box.Size, 0, Vector2.One));
                entity.AddComponent(new BoundingBoxComponent(box));

                //entity.AddComponent(new DrawBoundingBoxComponent { Color = Microsoft.Xna.Framework.Color.Red, IsEnabled = true });

                var hover = new MouseHoverComponent();
                hover.HoverChanged += (o, e) =>
                {
                    hardpoint.Alpha = hover.IsHover ? 1 : BaseAlpha;
                    hardpoint.Thickness = hover.IsHover ? 2 : 1;
                    if (hover.IsHover) _hoverEntities.Add(partEntity);
                    else _hoverEntities.Remove(partEntity);
                };
                entity.AddComponent(hover);
            }
        }

        public override void UnloadContent()
        {
            Instance = null;
            _mouse.ButtonUp -= HandleMouseButtonUp;
            _mouse.ButtonDown -= HandleMouseButtonDown;
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
            _hardpointSystem.IsEnabled = mode == EditorMode.Weapons;
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
            if (args.Script.StartsWith("Refit_ChangeMode"))
            {
                Document.GetElementById("availablePartsList").SetProperty("display", "none");
                foreach (var element in Document.GetElementById("navigation").Children)
                {
                    element.SetClass("selectedmode", false);
                }
                args.TargetElement.SetClass("selectedmode", true);
            }
        }
    }
}
