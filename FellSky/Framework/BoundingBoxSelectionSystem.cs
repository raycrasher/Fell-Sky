﻿using System;
using System.Collections.Generic;
using Artemis;
using FellSky.Framework;
using FellSky.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FellSky.Framework
{
    [Artemis.Attributes.ArtemisEntitySystem(ExecutionType = Artemis.Manager.ExecutionType.Synchronous, GameLoopType = Artemis.Manager.GameLoopType.Draw )]
    public class BoundingBoxSelectionSystem : Artemis.System.EntitySystem
    {
        private IMouseService _mouse;
        private bool _isClick;
        private Vector2 _mousePos;
        private bool _isMarqueeActive = false;
        private Entity _marqueeBoxEntity;
        private GenericDrawableComponent _marqueeBoxDrawable;
        private bool _isMouseDown;
        private Vector2 _marqueeBoxStart;

        public List<Entity> SelectedEntities { get; } = new List<Entity>();

        public int SelectionButton { get; set; } = 0;
        public bool MarqueeBoxSelectionEnabled { get; set; } = true;
        public Color MarqueeBoxColor { get; set; } = Color.LightGray;
        public float MarqueeBoxThickness { get; set; } = 1;

        public BoundingBoxSelectionSystem()
            : base(Aspect.All(typeof(Transform), typeof(BoundingBoxSelector)))
        { }

        public override void LoadContent()
        {
            base.LoadContent();
            _mouse = BlackBoard.GetService<IMouseService>();
            _mouse.ButtonDown += OnButtonDown;
            _mouse.ButtonUp += OnButtonUp;
            _mouse.Move += OnMouseMove;
            _marqueeBoxDrawable = new GenericDrawableComponent() { DrawFunction = DrawMarquee };
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            _mouse.ButtonDown -= OnButtonDown;
            _mouse.ButtonUp -= OnButtonUp;
            _mouse.Move -= OnMouseMove;
        }

        private void OnMouseMove(Point point)
        {
            _mousePos = _mouse.ScreenPosition;
        }

        private void OnButtonDown(Point point, int button)
        {
            if (button != SelectionButton) return;
            _isClick = true;
            _mousePos = _mouse.ScreenPosition;
            _isMouseDown = true;
        }

        private void OnButtonUp(Point point, int button)
        {
            if (button != SelectionButton) return;
            _isMouseDown = false;
            _mousePos = _mouse.ScreenPosition;
        }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            if (MarqueeBoxSelectionEnabled) DoMarqueeSelection(entities);
            else DoClickSelection(entities);
        }

        private void DoMarqueeSelection(IDictionary<int, Entity> entities)
        {
            if(!_isMarqueeActive && _isMouseDown)
            {
                _marqueeBoxEntity = EntityWorld.CreateEntity();
                _marqueeBoxEntity.AddComponent(_marqueeBoxDrawable);
                _marqueeBoxEntity.Refresh();
                _isMarqueeActive = true;
                _marqueeBoxStart = _mouse.ScreenPosition;
            }
            else if(_isMarqueeActive && !_isMouseDown)
            {
                if(Vector2.DistanceSquared(_marqueeBoxStart, _mousePos) < 2)
                {
                    DoClickSelection(entities);
                    return;
                }

                var camera = BlackBoard.GetEntry<Camera2D>(Camera2D.PlayerCameraName);
                var marqueeShape = new FarseerPhysics.Collision.Shapes.PolygonShape( FarseerPhysics.Common.PolygonTools.CreateRectangle(
                    Math.Abs(_marqueeBoxStart.X - _mouse.ScreenPosition.X) / 2,
                    Math.Abs(_marqueeBoxStart.Y - _mouse.ScreenPosition.Y) / 2), 1);

                var marqueeBoxCenter = camera.ScreenToCameraSpace(new Vector2(_marqueeBoxStart.X + _mouse.ScreenPosition.X, _marqueeBoxStart.Y + _mouse.ScreenPosition.Y) / 2);                

                var marqueeBoxRot = new FarseerPhysics.Common.Rot(0);
                var marqueeXForm = new FarseerPhysics.Common.Transform(
                    ref marqueeBoxCenter, ref marqueeBoxRot
                    );
                _marqueeBoxEntity.Delete();
                _isMarqueeActive = false;
                

                foreach (var entity in entities.Values)
                {
                    var box = entity.GetComponent<BoundingBoxSelector>();
                    var xform = entity.GetComponent<Transform>();
                    var matrix = Matrix.Invert(camera.GetViewMatrix(box.Parallax)) * Matrix.Invert(entity.GetWorldMatrix());

                    var itemShape = new FarseerPhysics.Collision.Shapes.PolygonShape(FarseerPhysics.Common.PolygonTools.CreateRectangle(
                        box.BoundingBox.Width/2,
                        box.BoundingBox.Height/2
                        ), 1);

                    var manifold = new FarseerPhysics.Collision.Manifold();
                    var itemCenter = xform.Position;
                    var itemRot = new FarseerPhysics.Common.Rot(xform.Rotation);
                    var itemXForm = new FarseerPhysics.Common.Transform(ref itemCenter, ref itemRot);

                    FarseerPhysics.Collision.Collision.CollidePolygons(ref manifold, marqueeShape, ref marqueeXForm, itemShape, ref itemXForm);
                    if (manifold.PointCount > 0)
                    {
                        box.IsSelected = true;
                        SelectedEntities.Add(entity);
                    }
                    else
                    {
                        box.IsSelected = false;
                        SelectedEntities.Remove(entity);
                    }
                }
            }
        }

        private void DrawMarquee(GraphicsDevice device, SpriteBatch batch, Entity entity)
        {
            var camera = BlackBoard.GetEntry<Camera2D>(Camera2D.PlayerCameraName);

            var mousePos = camera.ScreenToCameraSpace(_mouse.ScreenPosition);
            var start = camera.ScreenToCameraSpace(_marqueeBoxStart);
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

        private void DoClickSelection(IDictionary<int, Entity> entities)
        {
            if (_isClick)
            {
                var camera = BlackBoard.GetEntry<Camera2D>(Camera2D.PlayerCameraName);
                foreach (var entity in entities.Values)
                {
                    var box = entity.GetComponent<BoundingBoxSelector>();
                    if (!box.IsEnabled) continue;

                    var matrix = Matrix.Invert(camera.GetViewMatrix(box.Parallax)) * Matrix.Invert(entity.GetWorldMatrix());

                    Vector2 position = _mousePos;
                    Vector2.Transform(ref position, ref matrix, out position);
                    if (box.BoundingBox.Contains(position))
                    {
                        box.IsSelected = true;
                        SelectedEntities.Add(entity);
                    }
                    else
                    {
                        box.IsSelected = false;
                        SelectedEntities.Remove(entity);
                    }
                }
            }
            _isClick = false;
        }
    }
}
