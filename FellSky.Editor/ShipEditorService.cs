﻿using FellSky.Framework;
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
    public class ShipEditorService: System.ComponentModel.INotifyPropertyChanged
    {
        class PartEditorComponent : Artemis.Interface.IComponent { }

        private IMouseService _mouse;
        private MouseControlledTransformSystem _transformSystem;
        private EntityWorld _world;
        private GameServiceContainer _services;

        public List<Entity> SelectedPartEntities { get; private set; }
        public List<Entity> PartEntities { get; private set; }
        public Entity ShipEntity { get; private set; }
        public Entity TransformEntity { get; private set; }
        public Ship Ship { get; private set; }

        public CloneTransformAction CloneTransformAction { get; set; } = CloneTransformAction.Move;
        public TransformOrigin TransformOrigin { get; set; } = TransformOrigin.Centroid;

        public HullColorType SelectedHullColorType { get; set; }
        public Color HullColor { get; set; } = Color.White;
        public Color TrimColor { get; set; } = Color.CornflowerBlue;
        public Color BaseColor { get; set; } = Color.Gold;

        public ShipEditorService(GameServiceContainer services, Artemis.EntityWorld world)
        {
            _services = services;
            _mouse = _services.GetService<IMouseService>();
            _world = world;
            _transformSystem = world.SystemManager.GetSystem<MouseControlledTransformSystem>();
            SelectedPartEntities = _world.SystemManager.GetSystem<BoundingBoxSelectionSystem>().SelectedEntities;
            PropertyChanged += OnColorChanged;
        }

        public void RotateParts()
        {
            if (_transformSystem.Mode != MouseControlledTransformMode.Rotate)
            {
                StartTransformOnSelectedParts();
                _transformSystem.Mode = MouseControlledTransformMode.Rotate;
            }
        }

        public void TranslateParts()
        {
            if (_transformSystem.Mode != MouseControlledTransformMode.Translate)
            {
                StartTransformOnSelectedParts();
                _transformSystem.Mode = MouseControlledTransformMode.Translate;
            }
        }

        public void ScaleParts()
        {
            if (_transformSystem.Mode != MouseControlledTransformMode.Scale)
            {
                StartTransformOnSelectedParts();
                _transformSystem.Mode = MouseControlledTransformMode.Scale;
            }
        }

        public void DeleteParts()
        {
            var ship = ShipEntity.GetComponent<ShipComponent>();
            foreach (var e in SelectedPartEntities)
            {
                ship.RemovePart(e.Components.OfType<ShipPart>().First());
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

        public void AddHull(Sprite sprite)
        {
            ClearSelection();
            var entity = AddHullInternal(sprite.Id, Vector2.Zero, 0, Vector2.One, new Vector2(sprite.OriginX ?? sprite.W / 2, sprite.OriginY ?? sprite.H / 2), HullColor, SelectedHullColorType);
            SelectedPartEntities.Add(entity);
            _transformSystem.Mode = MouseControlledTransformMode.Translate;
            _transformSystem.Origin = Vector2.Zero;
            entity.AddComponent(new MouseControlledTransformComponent());
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
                else
                {
                    entity.GetComponent<MouseControlledTransformComponent>().InitialTransform = entity.GetComponent<Transform>().Clone();
                }
            }
        }

        public void MirrorLateralOnSelected()
        {
            //foreach(var item in SelectedPartEntities)
            //{
            //    var part = item.GetComponent<ShipPartComponent<Hull>>();
            //    if(part is Hull) MirrorHullLateral((Hull) part);
            //}
        }

        public void SaveShip(string filename)
        {

        }

        public void SaveAsPartGroup(string filename)
        {

        }

        private void MirrorHullLateral(Hull part)
        {
            Vector2 position = part.Transform.Position * new Vector2(1, -1);
            Vector2 scale = part.Transform.Scale;
            Vector2 origin = part.Transform.Origin;
            float rotation = (part.Transform.Rotation.ToVector() * new Vector2(-1, 1)).ToAngleRadians();
            
            if (Ship.Hulls.Any(h => Vector2.DistanceSquared(h.Transform.Position, position) < 1
                                    && Math.Abs(h.Transform.Rotation - rotation) < 1 / Math.PI))
                return;

            var oldHull = part as Hull;
            var hullEntity = AddHullInternal(oldHull.SpriteId, position, rotation, scale, origin, oldHull.Color, oldHull.ColorType);
            var newHull = hullEntity.Components.OfType<Hull>().First();
            newHull.SpriteEffect = oldHull.SpriteEffect ^ Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipVertically;
            newHull.ColorType = oldHull.ColorType;
        }
        
        private Entity AddHullInternal(string id, Vector2 position, float rotation, Vector2 scale, Vector2 origin, Color color, HullColorType colorType = HullColorType.Hull)
        {
            var hull = new Hull(id, position, rotation, scale, origin, color);
            var hullEntity = _services.GetService<ShipEntityFactory>().CreateHullEntity(ShipEntity, hull, false);
            var hullComponent = hullEntity.GetComponent<HullComponent>();
            hull.ColorType = colorType;
            hullEntity.AddComponent(new PartEditorComponent());
            var boundingbox = hullEntity.GetComponent<BoundingBoxComponent>();
            boundingbox.Box.Inflate(2, 2);

            var select = new BoundingBoxSelectorComponent() { IsEnabled = false };
            hullEntity.AddComponent(select);

            var drawbounds = new DrawBoundingBoxComponent();
            drawbounds.IsEnabled = false;
            hullEntity.AddComponent(drawbounds);
            select.SelectedChanged += (s, e) => drawbounds.IsEnabled = select.IsSelected;

            hullEntity.Refresh();
            return hullEntity;
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

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }
}