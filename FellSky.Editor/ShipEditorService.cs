using FellSky.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using Microsoft.Xna.Framework;
using FellSky.Ships;
using FellSky.Models.Ships.Parts;
using PropertyChanged;
using System.ComponentModel;
using FellSky.Components;
using FellSky.Systems;
using FellSky.Services;
using FellSky.EntityFactories;
using FellSky.Systems.MouseControlledTransformSystemStates;
using System.Windows;

namespace FellSky.Editor
{
    public enum CloneTransformAction
    {
        Move, Rotate, Scale
    }

    public enum TransformOrigin
    {
        Centroid, Local, Cursor
    }

    [ImplementPropertyChanged]
    public class ShipEditorService: INotifyPropertyChanged
    {
        class PartEditorComponent : Artemis.Interface.IComponent { }

        private IMouseService _mouse;
        private MouseControlledTransformSystem _transformSystem;
        private EntityWorld _world;
        private ShipEntityFactory _shipFactory;

        public List<Entity> SelectedPartEntities { get; private set; }
        public Entity ShipEntity { get; private set; }
        public Entity TransformEntity { get; private set; }
        public Ship Ship { get; private set; }

        public CloneTransformAction CloneTransformAction { get; set; } = CloneTransformAction.Move;
        public TransformOrigin TransformOrigin { get; set; } = TransformOrigin.Centroid;

        public HullColorType SelectedHullColorType { get; set; }
        public Color HullColor { get; set; } = Color.White;
        public Color TrimColor { get; set; } = Color.CornflowerBlue;
        public Color BaseColor { get; set; } = Color.Gold;
        public string CameraTag { get; set; }

        public bool IsSnapEnabled
        {
            get { return _transformSystem.IsSnapEnabled; }
            set { _transformSystem.IsSnapEnabled = value; }
        }

        public float SnapAmount
        {
            get { return _transformSystem.SnapAmount; }
            set { _transformSystem.SnapAmount = value; }
        }

        public Axis AxisConstraint
        {
            get { return (_transformSystem.State as TranslateState)?.Constraint ?? Axis.None; }
            set { if (_transformSystem.State is TranslateState) ((TranslateState)_transformSystem.State).Constraint = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mouse"></param>
        /// <param name="shipFactory"></param>
        /// <param name="world"></param>
        /// <param name="cameraTag">The tag of the camera entity</param>
        public ShipEditorService(IMouseService mouse, ShipEntityFactory shipFactory, Artemis.EntityWorld world, string cameraTag)
        {
            CameraTag = cameraTag;
            _mouse = mouse;
            _world = world;
            _shipFactory = shipFactory;
            _transformSystem = world.SystemManager.GetSystem<MouseControlledTransformSystem>();
            SelectedPartEntities = _world.SystemManager.GetSystem<BoundingBoxSelectionSystem>().SelectedEntities;
            PropertyChanged += OnColorChanged;
        }

        /// <summary>
        /// Starts a rotate operation
        /// </summary>
        public void RotateParts()
        {
            if (!(_transformSystem.State is RotateCentroidState))
            {
                _transformSystem.StartTransform<RotateCentroidState>();
                StartTransformOnSelectedParts();
            }
        }

        /// <summary>
        /// Starts a translate operation
        /// </summary>
        public void TranslateParts()
        {
            if (!(_transformSystem.State is TranslateState))
            {
                _transformSystem.StartTransform<TranslateState>();
                StartTransformOnSelectedParts();
            }
        }

        /// <summary>
        /// Not implemented yet.
        /// </summary>
        public void ScaleParts()
        {
            // not implemented yet
        }

        /// <summary>
        /// Delete parts
        /// </summary>
        public void DeleteParts()
        {
            var ship = ShipEntity.GetComponent<ShipComponent>();
            foreach (var e in SelectedPartEntities)
            {
                ship.RemovePart(e);
                if(e.HasComponent<HullComponent>())
                    ship.HullEntities.Remove(e);
                if (e.HasComponent<ThrusterComponent>())
                    ship.ThrusterEntities.Remove(e);
                e.Delete();
            }
            SelectedPartEntities.Clear();
        }

        public void OffsetParts(Vector2 offset)
        {
            foreach (var xform in SelectedPartEntities.Select(e => e.GetComponent<Transform>()).Where(t => t != null))
            {
                xform.Position += offset;
            }
        }

        /// <summary>
        /// Add a hull
        /// </summary>
        /// <param name="sprite"></param>
        public void AddHull(Sprite sprite)
        {
            ClearSelection();
            //_mouse.ScreenPosition = new Vector2(_mouse.ScreenPosition);

            var entity = AddHullInternal(sprite.Id, 
                //_world.GetCamera(CameraTag).ScreenToCameraSpace(_mouse.ScreenPosition), 
                Vector2.Zero,
                0, 
                Vector2.One, 
                new Vector2(
                    sprite.OriginX ?? sprite.W / 2, 
                    sprite.OriginY ?? sprite.H / 2), 
                HullColor, 
                SelectedHullColorType);

            SelectedPartEntities.Add(entity);
            entity.AddComponent(new MouseControlledTransformComponent());
            
            _transformSystem.StartTransform<TranslateState>();
        }

        public void ClearSelection()
        {
            foreach (var entity in SelectedPartEntities)
            {
                entity.GetComponent<BoundingBoxSelectorComponent>().IsSelected = false;
                entity.GetComponent<BoundingBoxSelectorComponent>().IsEnabled = true;
                entity.GetComponent<DrawBoundingBoxComponent>().IsEnabled = false;
                entity.RemoveComponent<MouseControlledTransformComponent>();
                entity.Refresh();
            }
            SelectedPartEntities.Clear();
        }

        public void CreateNewShip()
        {
            ClearSelection();
            foreach (var entity in _world.EntityManager.GetEntities(Aspect.All(typeof(PartEditorComponent))))
            {
                entity.Delete();
            }

            if (ShipEntity != null) ShipEntity.Tag = null;
            ShipEntity?.Delete();

            Ship = new Ship();
            ShipEntity = _world.CreateEntity();
            ShipEntity.AddComponent(new ShipComponent(Ship));
            ShipEntity.AddComponent(new Transform());
            ShipEntity.Refresh();
            ShipEntity.Tag = "PlayerShip";
        }

        private void StartTransformOnSelectedParts()
        {
            foreach (var entity in SelectedPartEntities)
            {
                if (!entity.HasComponent<MouseControlledTransformComponent>())
                {
                    entity.AddComponent(new MouseControlledTransformComponent());
                    entity.Refresh();
                }
            }
        }

        public void SaveShip(string filename)
        {
            try
            {
                Ship.SaveToJsonFile(filename);
            }
            catch (Newtonsoft.Json.JsonException)
            {
                MessageBox.Show($"There has been an error saving the ship to the following file: {filename}", "Error saving ship", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LoadShip(string fileName)
        {
            try
            {
                ClearSelection();
                foreach (var entity in _world.EntityManager.GetEntities(Aspect.All(typeof(PartEditorComponent))))
                {
                    entity.Delete();
                }
                if (ShipEntity != null) ShipEntity.Tag = null;
                ShipEntity?.Delete();
                Ship = Ship.LoadFromJsonFile(fileName);
                ShipEntity = _shipFactory.CreateShipEntity(Ship, Vector2.Zero, 0, false);
                ShipEntity.Tag = "PlayerShip";
                foreach(var entity in ShipEntity.GetComponent<ShipComponent>().ChildEntities)
                {
                    AddEditorComponentsToPartEntity(entity);
                }
            }
            catch (Newtonsoft.Json.JsonException)
            {
                MessageBox.Show($"There has been an error saving the ship to the following file: {fileName}", "Error saving ship", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SaveAsPartGroup(string filename)
        {

        }

        public void MirrorSelectedLaterally()
        {


            foreach(var part in SelectedPartEntities.Select(s => s.Components.OfType<IShipPartComponent>().First().Part))
            {
                Vector2 position = part.Transform.Position * new Vector2(1, -1);
                Vector2 scale = part.Transform.Scale;
                Vector2 origin = part.Transform.Origin;
                float rotation = (part.Transform.Rotation.ToVector() * new Vector2(-1, 1)).ToAngleRadians();

                if (Ship.Parts.Any(part2 => Vector2.DistanceSquared(part2.Transform.Position, position) < 1
                                        && Math.Abs(part2.Transform.Rotation - rotation) < 1 / Math.PI
                                        && part2.SpriteId == part2.SpriteId
                                        )) return;

                if (part is Hull)
                {
                    var oldHull = part as Hull;
                    var hullEntity = AddHullInternal(oldHull.SpriteId, position, rotation, scale, origin, oldHull.Color, oldHull.ColorType);
                    var newHull = hullEntity.GetComponent<HullComponent>().Part;
                    newHull.SpriteEffect = oldHull.SpriteEffect ^ Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipVertically;
                    newHull.ColorType = oldHull.ColorType;
                }
            }
        }
        
        private Entity AddHullInternal(string id, Vector2 position, float rotation, Vector2 scale, Vector2 origin, Color color, HullColorType colorType = HullColorType.Hull)
        {
            var hull = new Hull(id, position, rotation, scale, origin, color);
            var hullEntity = _shipFactory.CreateHullEntity(ShipEntity, hull, false);
            var hullComponent = hullEntity.GetComponent<HullComponent>();
            hull.ColorType = colorType;
            AddEditorComponentsToPartEntity(hullEntity);
            return hullEntity;
        }

        private void AddEditorComponentsToPartEntity(Entity entity)
        {
            entity.AddComponent(new PartEditorComponent());

            var select = new BoundingBoxSelectorComponent() { IsEnabled = false };
            entity.AddComponent(select);

            var drawbounds = new DrawBoundingBoxComponent();
            drawbounds.IsEnabled = false;
            entity.AddComponent(drawbounds);
            select.SelectedChanged += (s, e) => drawbounds.IsEnabled = select.IsSelected;

            entity.Refresh();
        }

        private void OnColorChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(HullColor))
            {
                foreach (var hull in SelectedPartEntities.Select(en => en.Components.OfType<Hull>().First()))
                    hull.Color = HullColor;
            }
            else if (e.PropertyName == nameof(BaseColor))
            {
                Ship.BaseDecalColor = BaseColor;
            }
            else if (e.PropertyName == nameof(TrimColor))
            {
                Ship.TrimDecalColor = TrimColor;
            }
            else if (e.PropertyName == nameof(SelectedHullColorType))
            {
                foreach (var hull in SelectedPartEntities.Select(en => en.Components.OfType<Hull>().First()))
                    hull.ColorType = SelectedHullColorType;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        
    }
}
