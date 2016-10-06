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
using System.Windows.Data;
using FellSky.Systems.SceneGraphRenderers;

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
        private IMouseService _mouse;
        private MouseControlledTransformSystem _transformSystem;
        private EntityWorld _world;

        public ObservableCollection<Entity> SelectedPartEntities => _world.SystemManager.GetSystem<BoundingBoxSelectionSystem>().SelectedEntities;
        public Entity ModelEntity { get; private set; }
        public Entity TransformEntity { get; private set; }
        public ShipModel Model { get; private set; }

        public CloneTransformAction CloneTransformAction { get; set; } = CloneTransformAction.Move;
        public TransformOrigin TransformOrigin { get; set; } = TransformOrigin.Centroid;

        public Color PartColor { get; set; } = Color.White;
        public Color TrimColor { get; set; } = Color.CornflowerBlue;
        public Color BaseColor { get; set; } = Color.Gold;

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

        public float WeaponArcAngle
        {
            get {
                return SelectedPartEntities
                  .Where(e => e.HasComponent<HardpointComponent>())
                  .Select(e => e.GetComponent<HardpointComponent>().Hardpoint)
                  .FirstOrDefault()?.Traverse ?? 0;
            }
            set
            {
                foreach(var item in SelectedPartEntities
                                    .Where(e => e.HasComponent<HardpointComponent>())
                                    .Select(e => e.GetComponent<HardpointComponent>().Hardpoint)
                                    )
                {
                    item.Traverse = value;
                }
            }
        }

        public static ShipEditorService Instance { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mouse"></param>
        /// <param name="shipFactory"></param>
        /// <param name="world"></param>
        /// <param name="cameraTag">The tag of the camera entity</param>
        public ShipEditorService(IMouseService mouse, Artemis.EntityWorld world)
        {
            _mouse = mouse;
            _world = world;
            _transformSystem = world.SystemManager.GetSystem<MouseControlledTransformSystem>();
            PropertyChanged += HandlePropertyChanged;
            SelectedPartEntities.CollectionChanged += OnSelectionChanged;
            Instance = this;
        }

        private void OnSelectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedPartEntities)));
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
            var component = ModelEntity.GetComponent<ShipModelComponent>();
            var parts = SelectedPartEntities.Select(pe => pe.GetComponent<IShipPartComponent>().Part);
            var model = component.Model;
            var hardpointDictionary = model?.Hardpoints.ToDictionary(s => (ShipPart)s.Hull);
            foreach (var item in parts)
            {
                component.Model.Parts.Remove(item);
                if (model != null && hardpointDictionary.ContainsKey(item))
                {
                    model.Hardpoints.Remove(hardpointDictionary[item]);
                }
            }

            foreach (var e in SelectedPartEntities)
            {
                e.Delete();
            }
            //ShipEntityFactory.UpdateComponentPartList(_world, ShipEntity, false);
            SelectedPartEntities.Clear();

            if (model != null)
                CollectionViewSource.GetDefaultView(model.Hardpoints).Refresh();
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
            var camera = _world.TagManager.GetEntity(Constants.ActiveCameraTag).GetComponent<Camera>();
            var entity = AddHullInternal(sprite.Id, 
                camera.ScreenToCameraSpace(_mouse.ScreenPosition), 
                //Vector2.Zero,
                0, 
                Vector2.One, 
                new Vector2(
                    sprite.OriginX ?? sprite.W / 2, 
                    sprite.OriginY ?? sprite.H / 2), 
                PartColor, 
                HullColorType.Hull);

            SelectedPartEntities.Add(entity);
            entity.AddComponent(new MouseControlledTransformComponent());
            
            _transformSystem.StartTransform<TranslateState>();
        }

        public void AddThruster(Sprite sprite)
        {
            ClearSelection();
            var camera = _world.TagManager.GetEntity(Constants.ActiveCameraTag).GetComponent<Camera>();
            var entity = AddThrusterInternal(sprite.Id,
                camera.ScreenToCameraSpace(_mouse.ScreenPosition),
                //Vector2.Zero, 
                0, 
                Vector2.One,
                new Vector2(
                    sprite.OriginX ?? sprite.W,
                    sprite.OriginY ?? sprite.H / 2),
                PartColor
                );
            SelectedPartEntities.Add(entity);
            entity.AddComponent(new MouseControlledTransformComponent());
            _transformSystem.StartTransform<TranslateState>();
        }

        public void AddDummyPart()
        {
            ClearSelection();
            var camera = _world.TagManager.GetEntity(Constants.ActiveCameraTag).GetComponent<Camera>();
            var entity = AddDummyPartInternal(camera.ScreenToCameraSpace(_mouse.ScreenPosition), 0, Vector2.One, Vector2.Zero);
            SelectedPartEntities.Add(entity);
            entity.AddComponent(new MouseControlledTransformComponent());
            _transformSystem.StartTransform<TranslateState>();
        }

        private Entity AddDummyPartInternal(Vector2 position, float rotation, Vector2 scale, Vector2 origin)
        {
            var part = new DummyPart();
            Model.Parts.Add(part);
            var entity = part.CreateEntity(_world, ModelEntity);
            entity.AddComponent(new EditorComponent());
            entity.AddComponent(new GenericDrawableComponent((a, b, e) => {
                var xform = e.GetComponent<Transform>();
                b.DrawCircle(xform.Position, 9, 15, Color.Cyan);
                b.DrawLine(xform.Position.X - 10, xform.Position.Y, xform.Position.X + 10, xform.Position.Y, Color.LightCyan);
                b.DrawLine(xform.Position.X, xform.Position.Y-10, xform.Position.X, xform.Position.Y+10, Color.LightCyan);

            }));
            AddEditorComponentsToPartEntity(entity);
            return entity;
        }

        public void ChangePartDepth(int delta)
        {
            var children = ModelEntity.GetChildren();
            if (SelectedPartEntities.Count > 0)
            {
                var parts = SelectedPartEntities
                    .Select(pe => new {
                        Part = pe.GetComponent<IShipPartComponent>().Part,
                        Index = Model.Parts.IndexOf(pe.GetComponent<IShipPartComponent>().Part),
                        SGIndex = children.IndexOf(pe)
                    })
                    .ToArray();
                foreach (var item in parts)
                {
                    Model.Parts.Move(item.Index, MathHelper.Clamp(item.Index + delta, 0, Model.Parts.Count - 1));
                    children.Move(item.SGIndex, MathHelper.Clamp(item.SGIndex + delta, 0, children.Count - 1));
                }
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

        public void CreateNewModel()
        {
            ClearSelection();
            foreach (var entity in _world.EntityManager.GetEntities(Aspect.All(typeof(EditorComponent))))
            {
                entity.Delete();
            }

            if (ModelEntity != null) ModelEntity.Tag = null;
            ModelEntity?.Delete();

            Model = new ShipModel();
            ModelEntity = Model.CreateStandAloneEntity(_world); ;
            ModelEntity.Tag = "PlayerShip";

            //PropertyObject = Model;
        }

        public void FlipLocal(SpriteEffects flip)
        {
            if (SelectedPartEntities.Count <= 0) return;
            foreach(var item in SelectedPartEntities.Select(e => e.GetComponent<IShipPartComponent>().Part))
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
            foreach (var item in SelectedPartEntities.Select(e => e.GetComponent<IShipPartComponent>().Part))
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

        public void ToggleHardpointOnSelected()
        {
            foreach(var item in SelectedPartEntities.Where(s=>s.HasComponent<HullComponent>()))
            {
                if (item.HasComponent<HardpointComponent>())
                {
                    var component = item.GetComponent<HardpointComponent>();
                    item.RemoveComponent<HardpointComponent>();
                    item.RemoveComponent<HardpointArcDrawingComponent>();
                    Model.Hardpoints.Remove(component.Hardpoint);
                }
                else
                {
                    var component = new HardpointComponent(new Hardpoint {
                        Traverse = MathHelper.ToRadians(60),
                        Hull = item.GetComponent<HullComponent>().Part,
                        Size = HardpointSize.Small,
                        Type = HardpointType.Universal
                    });
                    item.AddComponent(component);
                    item.AddComponent(new HardpointArcDrawingComponent());
                    Model.Hardpoints.Add(component.Hardpoint);
                }
            }
            CollectionViewSource.GetDefaultView(Model.Hardpoints).Refresh();
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

            var children = ModelEntity.GetChildren();

            Func<Entity, ShipPart, Entity> CreatePartEntity = (Entity oldEntity, ShipPart oldPart) =>
            {
                var part = oldPart.Clone();
                var entity = part.CreateEntity(_world, ModelEntity, children.IndexOf(oldEntity) + 1);
                Model.Parts.Insert(Model.Parts.IndexOf(oldPart) + 1, part);
                
                AddEditorComponentsToPartEntity(entity);
                return entity;
            };

            var toClone = SelectedPartEntities.ToArray();
            ClearSelection();
                        
            SelectedPartEntities.AddRange(from entity in toClone
                                    let part = entity.GetComponent<IShipPartComponent>().Part
                                    select CreatePartEntity(entity, part));

            foreach(var entity in SelectedPartEntities)
            {
                entity.GetComponent<BoundingBoxSelectorComponent>().IsSelected = true;
            }
            //ShipEntityFactory.UpdateComponentPartList(_world, ShipEntity, false);
            TranslateParts();
        }

        public void SaveShip(string filename)
        {
            try
            {
                Model.SaveToFile(filename);
            }
            catch (Newtonsoft.Json.JsonException)
            {
                MessageBox.Show($"There has been an error saving the ship to the following file: {filename}", "Error saving ship", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LoadShipModel(string fileName)
        {
            try
            {
                ClearSelection();
                foreach (var entity in _world.EntityManager.GetEntities(Aspect.All(typeof(EditorComponent))))
                {
                    entity.Delete();
                }
                if (ModelEntity != null) ModelEntity.Tag = null;
                ModelEntity?.Delete();
                Model = Persistence.LoadFromFile<ShipModel>(fileName);
                ModelEntity = Model.CreateStandAloneEntity(_world);
                ModelEntity.Tag = "PlayerShip";
                foreach (var entity in ModelEntity.GetChildren())
                {
                    AddEditorComponentsToPartEntity(entity);
                    if (entity.HasComponent<HardpointComponent>())
                        entity.AddComponent(new HardpointArcDrawingComponent());
                }
                //PropertyObject = Model;
            }
            catch (Newtonsoft.Json.JsonException)
            {
                MessageBox.Show($"There has been an error saving the ship to the following file: {fileName}", "Error saving ship", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void MirrorSelectedLaterally()
        {
            var children = ModelEntity.GetChildren();
            foreach (var oldEntity in SelectedPartEntities)
            {
                var part = oldEntity.GetComponent<IShipPartComponent>().Part;
                Vector2 position = part.Transform.Position * new Vector2(1, -1);
                Vector2 scale = part.Transform.Scale * new Vector2(1,-1);
                float rotation = (part.Transform.Rotation.ToVector() * new Vector2(-1, 1)).GetAngleRadians();

                if (Model.Parts.Any(part2 => Vector2.DistanceSquared(part2.Transform.Position, position) < 1
                                        && Math.Abs(part2.Transform.Rotation - rotation) < 1 / Math.PI
                                        && part2.SpriteId == part2.SpriteId
                                        )) return;

                var newPart = part.Clone();
                Model.Parts.Insert(Model.Parts.IndexOf(part) + 1, newPart);
                var entity = newPart.CreateEntity(_world, ModelEntity, children.IndexOf(oldEntity) + 1);
                AddEditorComponentsToPartEntity(entity);

                var xform = entity.GetComponent<Transform>();
                xform.Position = position;
                xform.Scale = scale;
                xform.Rotation = rotation;
            }            
        }
        
        private Entity AddHullInternal(string id, Vector2 position, float rotation, Vector2 scale, Vector2 origin, Color color, HullColorType colorType = HullColorType.Hull, int? index=null)
        {
            var hull = new Hull(id, position, rotation, scale, origin, color);

            Model.Parts.Add(hull);

            var hullEntity = hull.CreateEntity(_world, ModelEntity, index);
            var hullComponent = hullEntity.GetComponent<HullComponent>();
            hull.ColorType = colorType;
            AddEditorComponentsToPartEntity(hullEntity);
            return hullEntity;
        }

        private Entity AddThrusterInternal(string id, Vector2 position, float rotation, Vector2 scale, Vector2 origin, Color color, int? index=null)
        {
            var thruster = new Thruster(id, position, rotation, scale, origin, color);

            Model.Parts.Add(thruster);

            var thrusterEntity = thruster.CreateEntity(_world, ModelEntity, index);
            var thrusterComponent = thrusterEntity.GetComponent<ThrusterComponent>();
            thrusterComponent.ThrustPercentage = 1;
            AddEditorComponentsToPartEntity(thrusterEntity);
            return thrusterEntity;
        }

        private void AddEditorComponentsToPartEntity(Entity entity)
        {
            entity.AddComponent(new EditorComponent());

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

            entity.AddComponent(entity.GetComponent<IShipPartComponent>().Part.Transform);
            if (entity.HasComponent<DummyPartComponent>())
            {
                entity.AddComponent(new GenericDrawableComponent((a, b, e) => {
                    var xform = e.GetComponent<Transform>();
                    b.DrawCircle(xform.Position, 9, 15, Color.Cyan);
                    b.DrawLine(xform.Position.X - 10, xform.Position.Y, xform.Position.X + 10, xform.Position.Y, Color.LightCyan);
                    b.DrawLine(xform.Position.X, xform.Position.Y - 10, xform.Position.X, xform.Position.Y + 10, Color.LightCyan);
                }));
            }
            entity.Refresh();
        }

        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PartColor))
            {
                foreach (var part in SelectedPartEntities.Select(en => en.GetComponent<IShipPartComponent>()))
                    part.Part.Color = PartColor;
            }
            else if (e.PropertyName == nameof(BaseColor))
            {
                ModelEntity.GetComponent<ShipModelComponent>().BaseDecalColor = BaseColor;
            }
            else if (e.PropertyName == nameof(TrimColor))
            {
                ModelEntity.GetComponent<ShipModelComponent>().TrimDecalColor = TrimColor;
            }
        }

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067

        public void SetPartColor(Color color)
        {
            PartColor = color;
            foreach (var part in SelectedPartEntities.Select(e => e.GetComponent<IShipPartComponent>().Part))
                part.Color = color;
        }
    }
}
