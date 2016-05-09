using FellSky.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using Microsoft.Xna.Framework;
using FellSky.Game.Ships;
using FellSky.Game.Ships.Parts;
using PropertyChanged;
using System.ComponentModel;
using FellSky.Components;
using FellSky.Systems;
using FellSky.Services;
using FellSky.EntityFactories;
using FellSky.Systems.MouseControlledTransformSystemStates;
using System.Windows;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Xna.Framework.Graphics;

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

        public ObservableCollection<Entity> SelectedPartEntities => _world.SystemManager.GetSystem<BoundingBoxSelectionSystem>().SelectedEntities;
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

        public object PropertyObject { get; set; }

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
            get { return _transformSystem.State?.Constraint ?? Axis.None; }
            set { if(_transformSystem.State!=null) _transformSystem.State.Constraint = value; }
        }

        public static ShipEditorService Instance { get; private set; }

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
            PropertyChanged += OnColorChanged;
            SelectedPartEntities.CollectionChanged += OnSelectionChanged;
            Instance = this;
        }

        private void OnSelectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(SelectedPartEntities.Count > 0)
                PropertyObject = SelectedPartEntities[0].Components.OfType<IShipPartComponent>().First().Part;
            else
                PropertyObject = Ship;
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
        /// Scale parts
        /// </summary>
        public void ScaleParts()
        {
            if(!(_transformSystem.State is ScaleState))
            {
                _transformSystem.StartTransform<ScaleState>();
                StartTransformOnSelectedParts();
            }
        }

        /// <summary>
        /// Delete parts
        /// </summary>
        public void DeleteParts()
        {
            var shipComponent = ShipEntity.GetComponent<ShipComponent>();
            var parts = SelectedPartEntities.Select(pe => pe.Components.OfType<IShipPartComponent>().First().Part);

            foreach (var item in parts)
                shipComponent.Ship.Parts.Remove(item);

            foreach (var e in SelectedPartEntities)
            {
                e.Delete();
            }
            _shipFactory.UpdateShipComponentPartList(ShipEntity, false);
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
                _world.GetCamera(CameraTag).ScreenToCameraSpace(_mouse.ScreenPosition), 
                //Vector2.Zero,
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

        public void AddThruster(Sprite sprite)
        {
            ClearSelection();

            var entity = AddThrusterInternal(sprite.Id,
                _world.GetCamera(CameraTag).ScreenToCameraSpace(_mouse.ScreenPosition),
                //Vector2.Zero, 
                0, 
                Vector2.One,
                new Vector2(
                    sprite.OriginX ?? sprite.W,
                    sprite.OriginY ?? sprite.H / 2),
                HullColor
                );
            SelectedPartEntities.Add(entity);
            entity.AddComponent(new MouseControlledTransformComponent());
            _transformSystem.StartTransform<TranslateState>();
        }



        public void ChangePartDepth(int delta)
        {
            if (SelectedPartEntities.Count > 0)
            {
                var parts = SelectedPartEntities
                    .Select(pe => new { Part = pe.Components.OfType<IShipPartComponent>().First().Part, Index = Ship.Parts.IndexOf(pe.Components.OfType<IShipPartComponent>().First().Part) });
                foreach (var item in parts)
                {
                    Ship.Parts.Move(item.Index, MathHelper.Clamp(item.Index + delta, 0, Ship.Parts.Count - 1));
                }
                _shipFactory.UpdateShipComponentPartList(ShipEntity, false);
            }
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
            PropertyObject = Ship;
        }

        public void FlipLocal(SpriteEffects flip)
        {
            if (SelectedPartEntities.Count <= 0) return;
            foreach(var item in SelectedPartEntities.Select(e => e.Components.OfType<IShipPartComponent>().First().Part))
            {
                //if(item is Hull)
                //{
                //    var hull = (Hull)item;
                //    hull.SpriteEffect ^= fx;
                //}
                if (flip.HasFlag(SpriteEffects.FlipHorizontally)) item.Transform.Position *= new Vector2(-1, 1);
                if (flip.HasFlag(SpriteEffects.FlipVertically)) item.Transform.Position *= new Vector2(1, -1);
            }
        }

        public void FlipGroup(SpriteEffects flip)
        {
            if (flip == SpriteEffects.None) return;
            if (SelectedPartEntities.Count <= 0) return;
            var centroid = SelectedPartEntities.Aggregate(Vector2.Zero, (ta, e) => ta += e.GetComponent<Transform>().Position) / SelectedPartEntities.Count;
            foreach (var item in SelectedPartEntities.Select(e => e.Components.OfType<IShipPartComponent>().First().Part))
            {
                //if (item is Hull)
                //{
                //    var hull = (Hull)item;
                //    hull.SpriteEffect ^= flip;
                //}

                if (flip.HasFlag(SpriteEffects.FlipHorizontally)) item.Transform.Scale *= new Vector2(-1, 1);
                if (flip.HasFlag(SpriteEffects.FlipVertically)) item.Transform.Scale *= new Vector2(1, -1);

                if (flip == SpriteEffects.FlipVertically)
                {
                    item.Transform.Position = new Vector2(
                        item.Transform.Position.X,
                        item.Transform.Position.Y - (item.Transform.Position.Y - centroid.Y)*2
                        );
                } else if(flip == SpriteEffects.FlipHorizontally)
                {
                    item.Transform.Position = new Vector2(
                        item.Transform.Position.X - (item.Transform.Position.X - centroid.X) * 2,
                        item.Transform.Position.Y
                        );
                }

            }
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

        public void CloneParts()
        {
            if (SelectedPartEntities.Count <= 0) return;

            if (_transformSystem.State != null)
                _transformSystem.ApplyTransform();

            Func<ShipPart, Entity> CreatePartEntity = (ShipPart part) =>
            {
                var entity = _shipFactory.AddAndCreatePartEntity(ShipEntity, part.Clone(), false, Ship.Parts.IndexOf(part) + 1);
                AddEditorComponentsToPartEntity(entity);
                return entity;
            };

            var toClone = SelectedPartEntities.ToArray();
            ClearSelection();
                        
            SelectedPartEntities.AddRange(from entity in toClone
                                    let part = entity.Components.OfType<IShipPartComponent>().First().Part
                                    select CreatePartEntity(part));

            foreach(var entity in SelectedPartEntities)
            {
                entity.GetComponent<BoundingBoxSelectorComponent>().IsSelected = true;
            }
            _shipFactory.UpdateShipComponentPartList(ShipEntity, false);
            TranslateParts();
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
                foreach(var entity in ShipEntity.GetComponent<ShipComponent>().PartEntities)
                {
                    AddEditorComponentsToPartEntity(entity.Entity);
                }
                PropertyObject = Ship;
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
            foreach (var part in SelectedPartEntities.Select(s => s.Components.OfType<IShipPartComponent>().First().Part))
            {
                Vector2 position = part.Transform.Position * new Vector2(1, -1);
                Vector2 scale = part.Transform.Scale * new Vector2(1,-1);
                float rotation = (part.Transform.Rotation.ToVector() * new Vector2(-1, 1)).ToAngleRadians();

                if (Ship.Parts.Any(part2 => Vector2.DistanceSquared(part2.Transform.Position, position) < 1
                                        && Math.Abs(part2.Transform.Rotation - rotation) < 1 / Math.PI
                                        && part2.SpriteId == part2.SpriteId
                                        )) return;

                var entity = _shipFactory.AddAndCreatePartEntity(ShipEntity, part.Clone(), false, Ship.Parts.IndexOf(part) + 1);
                AddEditorComponentsToPartEntity(entity);

                var xform = entity.GetComponent<Transform>();
                xform.Position = position;
                xform.Scale = scale;
                xform.Rotation = rotation;
            }            
        }
        
        private Entity AddHullInternal(string id, Vector2 position, float rotation, Vector2 scale, Vector2 origin, Color color, HullColorType colorType = HullColorType.Hull, int index=-1)
        {
            var hull = new Hull(id, position, rotation, scale, origin, color);
            var hullEntity = _shipFactory.AddAndCreatePartEntity(ShipEntity, hull, false, index);
            var hullComponent = hullEntity.GetComponent<HullComponent>();
            hull.ColorType = colorType;
            AddEditorComponentsToPartEntity(hullEntity);
            return hullEntity;
        }

        private Entity AddThrusterInternal(string id, Vector2 position, float rotation, Vector2 scale, Vector2 origin, Color color, int index=-1)
        {
            var thruster = new Thruster(id, position, rotation, scale, origin, color);
            var thrusterEntity = _shipFactory.AddAndCreatePartEntity(ShipEntity, thruster, false, index);
            var thrusterComponent = thrusterEntity.GetComponent<ThrusterComponent>();
            thrusterComponent.ThrustPercentage = 1;
            AddEditorComponentsToPartEntity(thrusterEntity);
            return thrusterEntity;
        }

        private void AddEditorComponentsToPartEntity(Entity entity)
        {
            entity.AddComponent(new PartEditorComponent());

            var select = new BoundingBoxSelectorComponent() { IsEnabled = false };
            entity.AddComponent(select);

            var drawbounds = new DrawBoundingBoxComponent();
            drawbounds.IsEnabled = false;
            entity.AddComponent(drawbounds);
            select.SelectedChanged += (s, e) =>
            {
                drawbounds.IsEnabled = select.IsSelected;
                if (!select.IsSelected)
                    entity.RemoveComponent<MouseControlledTransformComponent>();
            };

            entity.Refresh();
        }

        private void OnColorChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(HullColor))
            {
                foreach (var hull in SelectedPartEntities.Select(en => en.Components.OfType<HullComponent>().First()))
                    hull.Part.Color = HullColor;
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
                foreach (var hull in SelectedPartEntities.Select(en => en.Components.OfType<HullComponent>().First()))
                    hull.Part.ColorType = SelectedHullColorType;
            }
        }

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067

        public void SetHullColor(Color color)
        {
            HullColor = color;
            foreach (var part in SelectedPartEntities.Select(e => e.Components.OfType<IShipPartComponent>().First().Part))
                part.Color = color;
        }
    }
}
