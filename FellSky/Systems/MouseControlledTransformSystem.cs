using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using FellSky.Framework;

using Microsoft.Xna.Framework;
using Artemis.Attributes;
using FellSky.Components;
using FellSky.Services;

namespace FellSky.Systems
{
    public enum MouseControlledTransformMode
    {
        Translate = 1, Rotate = 2, Scale = 3
    }

    public enum MouseRotateMode
    {
        Local, Centroid, Origin
    }

    [ArtemisEntitySystem(ExecutionType =Artemis.Manager.ExecutionType.Synchronous, GameLoopType =Artemis.Manager.GameLoopType.Update, Layer = 4)]
    public class MouseControlledTransformSystem : Artemis.System.EntityProcessingSystem
    {
        public Vector2 Origin { get; set; }

        public MouseRotateMode RotateMode {
            get { return _rotateMode; }
            set {
                if (_rotateMode != value) _modeChanged = true;
                _rotateMode = value;
            }
        } 

        //public override void OnChange(Entity entity) => GetInitialTransform(entity);

        public MouseControlledTransformMode? Mode {
            get { return _mode; }
            set {
                if (_mode != value) _modeChanged = true;
                _mode = value;
            }
        }

        public string CameraTag { get; set; }

        private MouseControlledTransformMode? _mode = null;
        private bool _modeChanged=false;
        private Vector2? _rotateOffset;
        private Vector2 _centroid;
        private MouseRotateMode _rotateMode = MouseRotateMode.Centroid;
        private CameraComponent _camera;
        private IMouseService _mouse;

        public MouseControlledTransformSystem(IMouseService mouseService, string cameraTag) 
            : base(Aspect.All(typeof(MouseControlledTransformComponent), typeof(Transform)))
        {
            _mouse = mouseService;
            CameraTag = cameraTag;
        }
        public override void OnAdded(Entity entity) => GetInitialTransform(entity);

        protected override void Begin()
        {
            base.Begin();
            _camera = EntityWorld.TagManager.GetEntity(CameraTag)?.GetComponent<CameraComponent>();
        }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            if(_modeChanged && Mode == MouseControlledTransformMode.Rotate && RotateMode == MouseRotateMode.Centroid)
            {
                _centroid = entities.Values.Aggregate(Vector2.Zero, (current, input) => current + input.GetComponent<Transform>().Position) / entities.Count;
            }
            base.ProcessEntities(entities);
        }

        public override void Process(Entity entity)
        {
            var control = entity.GetComponent<MouseControlledTransformComponent>();
            var transform = entity.GetComponent<Transform>();
            var worldMousePos = _camera.ScreenToCameraSpace(_mouse.ScreenPosition);
            Matrix? parentMatrix = null;
            if (_modeChanged)
            {
                Origin = worldMousePos;
                control.InitialTransform = transform.Clone();
            }
            parentMatrix = entity.GetComponent<LocalTransformComponent>()?.ParentWorldMatrix;
            switch (Mode)
            {
                case MouseControlledTransformMode.Translate:
                    DoTranslate(worldMousePos, control, transform, ref parentMatrix);
                    break;
                case MouseControlledTransformMode.Scale:
                    break;
                case MouseControlledTransformMode.Rotate:
                    switch (RotateMode)
                    {
                        case MouseRotateMode.Local:
                            DoRotateLocal(worldMousePos, control, transform, ref parentMatrix);
                            break;
                        case MouseRotateMode.Centroid:
                            DoRotateCentroid(worldMousePos, control, transform, _centroid, ref parentMatrix);
                            break;
                        case MouseRotateMode.Origin:
                            DoRotateOrigin(worldMousePos, control, transform, ref parentMatrix);
                            break;
                    }
                    
                    break;
            }
            _modeChanged = false;
        }

        private void DoRotateOrigin(Vector2 worldMousePos, MouseControlledTransformComponent control, Transform transform, ref Matrix? parentMatrix)
        {

            
        }

        private void DoRotateCentroid(Vector2 worldMousePos, MouseControlledTransformComponent control, Transform transform, Vector2 _centroid, ref Matrix? parent)
        {
            if (_modeChanged) _rotateOffset = null;

            if (Vector2.DistanceSquared(worldMousePos, _centroid) > 40)
                _rotateOffset = _rotateOffset ?? worldMousePos;

            if (_rotateOffset != null)
            {
                var initialAngle = (_rotateOffset.Value - _centroid).ToAngleRadians();
                var angle = (worldMousePos - _centroid).ToAngleRadians();

                var newMatrix =
                    Matrix.CreateScale(new Vector3(control.InitialTransform.Scale,0))
                    //* Matrix.CreateRotationZ(control.InitialTransform.Rotation)
                    * Matrix.CreateTranslation(new Vector3(control.InitialTransform.Position,0))
                    * Matrix.CreateTranslation(new Vector3(-_centroid, 0))
                    * Matrix.CreateRotationZ(MathHelper.WrapAngle(angle - initialAngle))
                    * Matrix.CreateTranslation(new Vector3(_centroid, 0));

                Vector2 position, scale;
                float rotation;
                Utilities.DecomposeMatrix2D(ref newMatrix, out position, out rotation, out scale);
                transform.Position = position;
                transform.Rotation = control.InitialTransform.Rotation + MathHelper.WrapAngle(angle - initialAngle);
                transform.Scale = scale;
            }

        }

        private void DoTranslate(Vector2 worldMousePos, MouseControlledTransformComponent control, Transform transform, ref Matrix? parent)
        {
            var offset = worldMousePos - Origin;
            //transform.Position = Vector2.Transform(control.InitialTransform.Position + offset, control.TransformationMatrix);
            transform.Position = parent != null ? Vector2.Transform(control.InitialTransform.Position + offset, parent.Value) : worldMousePos;
        }

        private void DoRotateLocal(Vector2 worldMousePos, MouseControlledTransformComponent control, Transform transform, ref Matrix? parent)
        {
            
            if(_modeChanged) _rotateOffset = null;

            if(_rotateOffset!=null)
            {
                var offset = worldMousePos - (parent != null ? Vector2.Transform(transform.Position, parent.Value) : transform.Position);
                var initialAngle = (_rotateOffset.Value - control.InitialTransform.Position).ToAngleRadians();

                transform.Rotation = MathHelper.WrapAngle(offset.ToAngleRadians() - initialAngle);
            }
            else { 
                if(Vector2.DistanceSquared(Origin, worldMousePos) > 40)
                    _rotateOffset = worldMousePos;
            }
        }

        private void GetInitialTransform(Entity entity)
        {
            var xform = entity.GetComponent<Transform>();
            var component = entity.GetComponent<MouseControlledTransformComponent>();
            component.InitialTransform = xform.Clone();
            //Origin = _camera.ScreenToCameraSpace(_mouse.ScreenPosition);
        }
    }
}
