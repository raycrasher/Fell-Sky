using System;
using System.Collections.Generic;
using Artemis;
using FellSky.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FellSky.Services;
using FellSky.Components;
using System.Collections.ObjectModel;

namespace FellSky.Systems
{
    public class BoundingBoxSelectionSystem : Artemis.System.EntitySystem
    {
        private IMouseService _mouse;
        private bool _isMarqueeActive = false;
        private Entity _marqueeBoxEntity;
        private GenericDrawableComponent _marqueeBoxDrawable;
        private bool _isMouseDown;
        private Vector2 _marqueeBoxStart;

        public ObservableCollection<Entity> SelectedEntities { get; } = new ObservableCollection<Entity>();

        public int SelectionButton { get; set; } = 0;
        public bool MarqueeBoxSelectionEnabled { get; set; } = true;
        public Color MarqueeBoxColor { get; set; } = Color.LightGray;
        public float MarqueeBoxThickness { get; set; } = 1;
        public string CameraTag { get; set; }

        public BoundingBoxSelectionSystem(IMouseService mouse, string cameraTag)
            : base(Aspect.All(typeof(Transform), typeof(BoundingBoxSelectorComponent)))
        {
            _mouse = mouse;
            CameraTag = cameraTag;
        }

        public override void LoadContent()
        {
            base.LoadContent();
            _mouse.ButtonDown += OnButtonDown;
            _mouse.ButtonUp += OnButtonUp;
            _marqueeBoxDrawable = new GenericDrawableComponent(DrawMarquee);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            _mouse.ButtonDown -= OnButtonDown;
            _mouse.ButtonUp -= OnButtonUp;
        }

        private void OnButtonDown(Point point, int button)
        {
            if (button != SelectionButton) return;
            _isMouseDown = true;
        }

        private void OnButtonUp(Point point, int button)
        {
            if (button != SelectionButton) return;
            _isMouseDown = false;
        }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            DoMarqueeSelection(entities);
        }

        private void DoMarqueeSelection(IDictionary<int, Entity> entities)
        {
            var camera = EntityWorld.GetCamera(CameraTag);
            var mousePos = camera.ScreenToCameraSpace(_mouse.ScreenPosition);

            if (!_isMarqueeActive && _isMouseDown)
            {
                _marqueeBoxEntity = EntityWorld.CreateEntity();
                _marqueeBoxEntity.AddComponent(_marqueeBoxDrawable);
                _marqueeBoxEntity.Refresh();
                _isMarqueeActive = true;
                _marqueeBoxStart = mousePos;
            }
            else if(_isMarqueeActive && !_isMouseDown)
            {
                bool clickMode = false;
                if(Vector2.DistanceSquared(_marqueeBoxStart, mousePos) < 2)
                {
                    clickMode = true;
                }
                
                if (Math.Abs(_marqueeBoxStart.X - mousePos.X) / 2 < float.Epsilon) 
                    mousePos.X = _marqueeBoxStart.X + 1;
                if (Math.Abs(_marqueeBoxStart.Y - mousePos.Y) / 2 < float.Epsilon)
                    mousePos.Y = _marqueeBoxStart.Y + 1;

                var marqueeShape = new FarseerPhysics.Collision.Shapes.PolygonShape( FarseerPhysics.Common.PolygonTools.CreateRectangle(
                    Math.Abs(_marqueeBoxStart.X - mousePos.X) / 2,
                    Math.Abs(_marqueeBoxStart.Y - mousePos.Y) / 2), 1);

                var marqueeBoxCenter = new Vector2(_marqueeBoxStart.X + mousePos.X, _marqueeBoxStart.Y + mousePos.Y) / 2;                

                var marqueeBoxRot = new FarseerPhysics.Common.Rot(0);
                var marqueeXForm = new FarseerPhysics.Common.Transform(
                    ref marqueeBoxCenter, ref marqueeBoxRot
                    );
                _marqueeBoxEntity.Delete();
                _isMarqueeActive = false;
                

                foreach (var entity in entities.Values)
                {
                    var select = entity.GetComponent<BoundingBoxSelectorComponent>();
                    var box = entity.GetComponent<BoundingBoxComponent>();
                    var xform = entity.GetComponent<Transform>();
                    var matrix = Matrix.Invert(camera.GetViewMatrix(select.Parallax)) * Matrix.Invert(entity.GetWorldMatrix());

                    // perform polygon test with oriented bounding boxes
                    var itemShape = new FarseerPhysics.Collision.Shapes.PolygonShape(FarseerPhysics.Common.PolygonTools.CreateRectangle(
                        box.Box.Width/2,
                        box.Box.Height/2
                        ), 1);

                    var manifold = new FarseerPhysics.Collision.Manifold();
                    var itemCenter = xform.Position;
                    var itemRot = new FarseerPhysics.Common.Rot(xform.Rotation);
                    var itemXForm = new FarseerPhysics.Common.Transform(ref itemCenter, ref itemRot);

                    FarseerPhysics.Collision.Collision.CollidePolygons(ref manifold, marqueeShape, ref marqueeXForm, itemShape, ref itemXForm);
                    if (manifold.PointCount > 0)
                    {
                        if (clickMode)
                        {
                            return;
                        }
                        select.IsSelected = true;
                        SelectedEntities.Add(entity);
                        
                    }
                    else
                    {
                        select.IsSelected = false;
                        SelectedEntities.Remove(entity);
                    }
                }
            }
        }

        private void DrawMarquee(GraphicsDevice device, SpriteBatch batch, Entity entity)
        {
            var camera = EntityWorld.GetCamera(CameraTag);

            var mousePos = camera.ScreenToCameraSpace(_mouse.ScreenPosition);
            var start = _marqueeBoxStart;
            Primitives2D.DrawRectangle(
                batch,
                new Vector2(Math.Min(start.X, mousePos.X),
                            Math.Min(start.Y, mousePos.Y)),
                new Vector2(Math.Abs(start.X - mousePos.X),
                            Math.Abs(start.Y - mousePos.Y)),
                MarqueeBoxColor,
                MarqueeBoxThickness
                );
        }

        
    }
}
