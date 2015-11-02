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
    [ArtemisEntitySystem(ExecutionType =Artemis.Manager.ExecutionType.Synchronous, GameLoopType =Artemis.Manager.GameLoopType.Update, Layer = 4)]
    public class MouseControlledTransformSystem : Artemis.System.EntityComponentProcessingSystem<MouseControlledTransformComponent, Transform>
    {
        private Camera2D _camera;
        private IMouseService _mouse;

        public override void LoadContent()
        {
            var provider = BlackBoard.GetEntry<IServiceProvider>("ServiceProvider");
            _mouse = provider.GetService<IMouseService>();
            _camera = BlackBoard.GetEntry<Camera2D>(Camera2D.PlayerCameraName);
            base.LoadContent();
        }
        public override void Process(Entity entity, MouseControlledTransformComponent control, Transform xform)
        {
            var worldPosition = _camera.ScreenToCameraSpace(_mouse.ScreenPosition);
            xform.Position = Vector2.Transform(worldPosition, control.TransformationMatrix);
        }
    }
}
