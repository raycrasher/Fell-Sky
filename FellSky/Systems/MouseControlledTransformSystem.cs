using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using FellSky.EntityComponents;
using FellSky.Framework;
using FellSky.Graphics;
using Microsoft.Xna.Framework;
using Artemis.Attributes;

namespace FellSky.Systems
{
    public enum MouseControlledTransformMode
    {
        Translate = 1, Rotate = 2, Scale = 3
    }

    [ArtemisEntitySystem(ExecutionType =Artemis.Manager.ExecutionType.Synchronous, GameLoopType =Artemis.Manager.GameLoopType.Update, Layer = 4)]
    public class MouseControlledTransformSystem : Artemis.System.EntityComponentProcessingSystem<MouseControlledTransformComponent, Transform>
    {
        private Camera2D _camera;
        private IMouseService _mouse;

        public override void OnAdded(Entity entity) => GetInitialTransform(entity);
        //public override void OnChange(Entity entity) => GetInitialTransform(entity);

        public MouseControlledTransformMode Mode { get; set; } = MouseControlledTransformMode.Translate;

        private void GetInitialTransform(Entity entity)
        {
            var xform = entity.GetComponent<Transform>();
            var component = entity.GetComponent<MouseControlledTransformComponent>();
            component.InitialTransform = xform.Clone();
            component.InitialMousePosition = _camera.ScreenToCameraSpace(_mouse.ScreenPosition);
        }

        public override void LoadContent()
        {
            var provider = BlackBoard.GetEntry<IServiceProvider>("ServiceProvider");
            _mouse = provider.GetService<IMouseService>();
            _camera = BlackBoard.GetEntry<Camera2D>(Camera2D.PlayerCameraName);
            base.LoadContent();
        }

        public override void Process(Entity entity, MouseControlledTransformComponent control, Transform transform)
        {
            var worldMousePos = _camera.ScreenToCameraSpace(_mouse.ScreenPosition);
            switch (Mode)
            {
                case MouseControlledTransformMode.Translate:
                    transform.Position = Vector2.Transform(worldMousePos, control.TransformationMatrix);
                    break;
                case MouseControlledTransformMode.Scale:
                    break;
                case MouseControlledTransformMode.Rotate:
                    DoRotate(worldMousePos, control, transform);
                    break;
            }
        }

        private void DoRotate(Vector2 worldMousePos, MouseControlledTransformComponent control, Transform transform)
        {
            var offset = Vector2.Transform(worldMousePos, control.TransformationMatrix) - transform.Position;
            var initialAngle = (control.InitialMousePosition - control.InitialTransform.Position).ToAngleRadians();

            transform.Rotation = MathHelper.WrapAngle(control.InitialTransform.Rotation + initialAngle);
        }
    }
}
