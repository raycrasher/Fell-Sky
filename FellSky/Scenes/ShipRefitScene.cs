﻿using FellSky.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibRocketNet;
using Microsoft.Xna.Framework;
using Artemis;
using FellSky.Systems;

using Microsoft.Xna.Framework.Graphics;
using FellSky.Game.Ships;
using FellSky.Components;
using FellSky.Game.Combat;
using FellSky.Framework;
using FellSky.Systems.SceneGraphRenderers;
using FellSky.Game.Ships.Modules;

namespace FellSky.Scenes
{
    public class ShipRefitScene: Scene
    {
        public enum EditorMode
        {
            Weapons, Modules, Armor, Build, Simulator
        }

        public const string RefitScreenGuiDocument = "Gui/RefitScreen.xml";

        class FleetEntry
        {
            public Entity OrigEntity;
            public ShipVariant Variant;
        }

        private readonly IGuiService GuiService;
        private GraphicsDevice Graphics;
        private HardpointRendererSystem _hardpointSystem;
        private IMouseService _mouse;
        private Point _lastClickPos;

        public EditorMode Mode { get; private set; }

        public ElementDocument Document { get; private set; }
        public static ShipRefitScene Instance { get; private set; }
        private List<FleetEntry> Fleet;

        public List<Texture2D> LoadedTextures { get; set; }
        public Entity CurrentShip { get; private set; }
        public Entity GridEntity { get; private set; }
        public Entity CameraEntity { get; private set; }
        public Scene PreviousState { get; set; }

        public ShipRefitScene(IReadOnlyList<Entity> fleet, EntityWorld originalWorld)
        {
            if (fleet == null) throw new ArgumentException("Fleet is null.");
            if (fleet.Count < 0) throw new ArgumentException("Fleet is empty.");
            Fleet = new List<FleetEntry>();

            OrigWorld = originalWorld;

            foreach (var entity in fleet)
            {
                var shipComponent = entity.GetComponent<ShipComponent>();
                if (shipComponent == null) continue;
                if (shipComponent.Variant == null)
                    shipComponent.Variant = CreateVariantFromEntity(entity);
                Fleet.Add(new FleetEntry {
                    Variant = shipComponent.Variant,
                    OrigEntity = entity
                });
            }

            GuiService = ServiceLocator.Instance.GetService<IGuiService>();
            _mouse = ServiceLocator.Instance.GetService<IMouseService>();
        }

        private ShipVariant CreateVariantFromEntity(Entity entity)
        {
            var shipComponent = entity.GetComponent<ShipComponent>();
            var variant = new ShipVariant();
            variant.HullId = shipComponent.Ship.Id;
            variant.Weapons = shipComponent.Hardpoints.Select(h =>
            {
                var hpc = h.GetComponent<HardpointComponent>();
                if (hpc == null) return null;
                return new
                {
                    HardpointId = hpc.Hardpoint.Id,
                    Weapon = hpc.InstalledEntity?.GetComponent<WeaponComponent>()?.Weapon.Id
                };
            }).ToDictionary(k => k.HardpointId, v => v.Weapon);
            //Modules=      // TODO
            variant.BaseDecalColor = entity.GetComponent<ShipModelComponent>().BaseDecalColor;
            variant.TrimDecalColor = entity.GetComponent<ShipModelComponent>().TrimDecalColor;
            variant.VariantName = $"{shipComponent.Ship.HullClass} Custom";
            return variant;
        }

        public override void LoadContent()
        {
            Instance = this;

            int depth = 0;

            World.CreateComponentPool<BulletComponent>();

            World.SystemManager.SetSystem(new GridRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new BackgroundRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            //World.SystemManager.SetSystem(new ShipRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new SceneGraphRendererSystem<StandardShipModelRenderer>(new StandardShipModelRenderer()), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new BeamRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new BulletRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new ParticleSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new BoundingBoxRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new GenericDrawableRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            World.SystemManager.SetSystem(new PhysicsDebugDrawSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            _hardpointSystem = World.SystemManager.SetSystem(new HardpointRendererSystem(), Artemis.Manager.GameLoopType.Draw, depth++);
            //World.SystemManager.SetSystem(new GuiSystem(), Artemis.Manager.GameLoopType.Draw, depth++);

            int priority = 1;
            World.SystemManager.SetSystem(new EventSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new CameraControlSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new RigidBodySystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new MouseControlledTransformSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new BoundingBoxSelectionSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new MouseHoverSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new CoroutineSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new PlayerShipControlSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new BeamSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new BulletSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new ShipUpdateSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            //World.SystemManager.SetSystem(new StorySystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new PhysicsSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new TurretRotationSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new WeaponSystem(), Artemis.Manager.GameLoopType.Update, priority++);
            World.SystemManager.SetSystem(new FrameAnimationSystem(), Artemis.Manager.GameLoopType.Update, priority++);


            Graphics = ServiceLocator.Instance.GetService<GraphicsDevice>();
            World.InitializeAll();

            CameraEntity = World.CreateCamera(Constants.ActiveCameraTag, Graphics);
            GridEntity = World.CreateGrid(new Vector2(100), new Microsoft.Xna.Framework.Color(5, 5, 10));

            if (Document == null)
            {
                Document = GuiService.Context.LoadDocument(RefitScreenGuiDocument);
                Core.ScriptEvent += (o,e) => Instance?.HandleScriptEvent(o, e);
            }

            CurrentShip = World.CreateShip(Fleet[0].Variant, Vector2.Zero, 0);

            //CurrentShip = World.CreateShip("Jaeger", Vector2.Zero, 0f, physics:true);
            //debug
            AddHardpointMarkersToShip(CurrentShip);

            SetMode(EditorMode.Weapons);
            
            _mouse.ButtonDown += HandleMouseButtonDown;
            _mouse.ButtonUp += HandleMouseButtonUp;

            Document.GetElementById("availablePartsList").SetProperty("display", "none");

            base.LoadContent();
        }

        private void HandleMouseButtonDown(Point pos, int button)
        {
            _lastClickPos = pos;

            if (Mode == EditorMode.Weapons && button == 0 && _hoverEntities.Count > 0)
                ShowAvailableWeaponsList(_hoverEntities.First());
            
        }

        private void ShowAvailableWeaponsList(Entity hardpointEntity)
        {
            _selectedHardpoint = hardpointEntity;
            var hardpoint = hardpointEntity.GetComponent<HardpointComponent>();
            var partsList = Document.GetElementById("availablePartsList");
            partsList.SetProperty("display", "block");
            partsList.RemoveAllChildren();

            var weapons = GetAvailableWeaponsForHardpoint(hardpointEntity);

            foreach(var weapon in weapons)
            {
                var wpnElement = new Element("weapon");
                var name = new Element("name");
                name.InnerRml = weapon.Name;
                wpnElement.AppendChild(name);
                var desc = new Element("description");
                desc.InnerRml = weapon.Description;
                wpnElement.AppendChild(desc);
                
                wpnElement.Click += (o, e) =>
                {
                    UninstallWeapon();
                    InstallWeapon(weapon.Id);

                };

                partsList.AppendChild(wpnElement);
            }
        }

        private IList<Weapon> GetAvailableWeaponsForHardpoint(Entity entity)
        {
            var hp = entity.GetComponent<HardpointComponent>();
            return CombatEntityFactory.Weapons.Values
                .Where(w=>w.CanInstallToHardpoint(CurrentShip, hp.Hardpoint))
                .ToList();
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

        private readonly HashSet<Entity> _hoverEntities = new HashSet<Entity>();
        private Entity _selectedHardpoint;
        private Action RunOnce;
        private EntityWorld OrigWorld;

        private void AddHardpointMarkersToShip(Entity ship)
        {
            var shipComponent = ship.GetComponent<ShipComponent>();
            
            const float BaseAlpha = 0.3f;
            foreach (var hardpointEntity in ship.GetChildren().Where(e=>e.HasComponent<HardpointComponent>())){
                var hardpointArc = new HardpointArcDrawingComponent { DrawHardpointIcon = true, Alpha = BaseAlpha };
                hardpointEntity.AddComponent(hardpointArc);
                var entity = World.CreateEntity();
                var xform = hardpointEntity.GetComponent<Transform>();
                var box = HardpointRendererSystem.GetIconBoundingBox(hardpointEntity.GetComponent<HardpointComponent>().Hardpoint);
                var newxform = entity.AddComponentFromPool<Transform>();

                newxform.Position = xform.Position - box.Size / 2;
                newxform.Rotation = 0;
                newxform.Scale = Vector2.One;

                entity.AddComponent(new BoundingBoxComponent(box));

                //entity.AddComponent(new DrawBoundingBoxComponent { Color = Microsoft.Xna.Framework.Color.Red, IsEnabled = true });

                var hover = new MouseHoverComponent { UsePositionOnly = true };
                hover.HoverChanged += (o, e) =>
                {
                    hardpointArc.Alpha = hover.IsHover ? 1 : BaseAlpha;
                    hardpointArc.Thickness = hover.IsHover ? 2 : 1;
                    if (hover.IsHover) _hoverEntities.Add(hardpointEntity);
                    else _hoverEntities.Remove(hardpointEntity);
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
            RunOnce?.Invoke();
            RunOnce = null;
            World.Update(gameTime.ElapsedGameTime.Milliseconds);
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
            
            if(mode == EditorMode.Simulator)
            {
                CurrentShip.Tag = "PlayerShip";
            }
            else
            {
                CurrentShip.Tag = null;
                CurrentShip.SetRigidBodyTransform(Vector2.Zero, 0);
                var body = CurrentShip.GetComponent<RigidBodyComponent>().Body;
                body.AngularVelocity = 0;
                body.LinearVelocity = Vector2.Zero;
                foreach (var turret in CurrentShip.GetComponent<ShipComponent>().Weapons.Select(w=>w.GetComponent<WeaponComponent>().Turret.GetComponent<TurretComponent>()))
                {
                    turret.Rotation = 0;
                    turret.DesiredRotation = 0;
                }

            }
            
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
                case "Refit_ChangeMode_RunSim":
                    SetMode(EditorMode.Simulator);
                    break;
                case "Refit_CloseScene":
                    if (PreviousState != null)
                    {
                        GameEngine.Instance.CurrentScene = PreviousState;
                        foreach(var item in Fleet)
                        {
                            item.Variant.ApplyVariant(OrigWorld, item.OrigEntity);
                        }
                        return;
                    }
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

        private void InstallWeapon(string id)
        {
            CombatEntityFactory.Weapons[id].Install(World, CurrentShip, _selectedHardpoint);
            Document.GetElementById("availablePartsList").SetProperty("display", "none");
        }

        private void UninstallWeapon()
        {
            var shipComponent = CurrentShip.GetComponent<ShipComponent>();
            var hardpoint = _selectedHardpoint;
            hardpoint.GetComponent<HardpointComponent>().InstalledEntity?.GetComponent<WeaponComponent>()?.Weapon.Uninstall(CurrentShip, hardpoint.GetComponent<HardpointComponent>().InstalledEntity);
        }

        public override void Enter(Scene previous)
        {
            Document.Show(ElementDocument.FocusFlags.None);
            Document.PullToFront();
        }

        public override void Exit(Scene next)
        {
            Document.Hide();
        }
    }
}
