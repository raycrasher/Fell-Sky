using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using FellSky.Framework;
using FellSky.Graphics;
using Microsoft.Xna.Framework;
using Artemis.Attributes;

namespace FellSky.Framework
{
    public enum MouseControlledTransformMode
    {
        Translate = 1, Rotate = 2, Scale = 3
    }

    [ArtemisEntitySystem(ExecutionType =Artemis.Manager.ExecutionType.Synchronous, GameLoopType =Artemis.Manager.GameLoopType.Update, Layer = 4)]
    public class MouseControlledTransformSystem : Artemis.System.EntityComponentProcessingSystem<MouseControlledTransform, Transform>
    {
        private Camera2D _camera;
        private IMouseService _mouse;
        public Vector2 Origin { get; set; }

        public override void OnAdded(Entity entity) => GetInitialTransform(entity);

        //public override void OnChange(Entity entity) => GetInitialTransform(entity);

        public MouseControlledTransformMode? Mode {
            get { return _mode; }
            set {
                if (_mode != value) _modeChanged = true;
                _mode = value;
            }
        }

        private MouseControlledTransformMode? _mode = null;
        private bool _modeChanged=false;
        private Vector2? _rotateOffset;

        private void GetInitialTransform(Entity entity)
        {
            var xform = entity.GetComponent<Transform>();
            var component = entity.GetComponent<MouseControlledTransform>();
            component.InitialTransform = xform.Clone();
            //Origin = _camera.ScreenToCameraSpace(_mouse.ScreenPosition);
        }

        public override void LoadContent()
        {
            _mouse = BlackBoard.GetService<IMouseService>();
            base.LoadContent();
        }

        protected override void Begin()
        {
            base.Begin();
            _camera = BlackBoard.GetEntry<Camera2D>(Camera2D.PlayerCameraName);
        }

        public override void Process(Entity entity, MouseControlledTransform control, Transform transform)
        {
            var worldMousePos = _camera.ScreenToCameraSpace(_mouse.ScreenPosition);
            Matrix? parentMatrix = null;
            if (_modeChanged)
            {
                Origin = worldMousePos;
                control.InitialTransform = transform.Clone();
            }
            parentMatrix = entity.GetComponent<ChildEntity>()?.ParentWorldMatrix;
            switch (Mode)
            {
                case MouseControlledTransformMode.Translate:
                    DoTranslate(worldMousePos, control, transform, ref parentMatrix);
                    break;
                case MouseControlledTransformMode.Scale:
                    break;
                case MouseControlledTransformMode.Rotate:
                    DoRotate(worldMousePos, control, transform, ref parentMatrix);
                    break;
            }
            _modeChanged = false;
        }

        private void DoTranslate(Vector2 worldMousePos, MouseControlledTransform control, Transform transform, ref Matrix? parent)
        {
            var offset = worldMousePos - Origin;
            //transform.Position = Vector2.Transform(control.InitialTransform.Position + offset, control.TransformationMatrix);
            transform.Position = parent != null ? Vector2.Transform(worldMousePos, parent.Value) : worldMousePos;
        }

        private void DoRotate(Vector2 worldMousePos, MouseControlledTransform control, Transform transform, ref Matrix? parent)
        {
            if(_modeChanged) _rotateOffset = null;

            if(_rotateOffset!=null)
            {
                var offset = worldMousePos - (parent != null ? Vector2.Transform(transform.Position, parent.Value) : transform.Position);
                var initialAngle = (_rotateOffset.Value - control.InitialTransform.Position).ToAngleRadians();

                transform.Rotation = -MathHelper.WrapAngle(initialAngle - offset.ToAngleRadians());
            }
            else { 
                if(Vector2.DistanceSquared(Origin, worldMousePos) > 40)
                    _rotateOffset = worldMousePos;
            }
        }
    }
}
