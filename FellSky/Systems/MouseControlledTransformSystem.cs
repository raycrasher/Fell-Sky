using System.Collections.Generic;
using Artemis;
using FellSky.Components;
using FellSky.Services;
using FellSky.Systems.MouseControlledTransformSystemStates;

namespace FellSky.Systems
{
    public class MouseControlledTransformSystem : Artemis.System.EntitySystem
    {
        public string CameraTag { get; set; }

        private IMouseService _mouse;
        public IMouseControlledTransformSystemState State { get; private set; }

        public MouseControlledTransformSystem(IMouseService mouseService, string cameraTag) 
            : base(Aspect.All(typeof(MouseControlledTransformComponent), typeof(Transform)))
        {
            _mouse = mouseService;
            CameraTag = cameraTag;
        }
       
        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            var camera = EntityWorld.TagManager.GetEntity(CameraTag)?.GetComponent<CameraComponent>();
            var worldMousePos = camera.ScreenToCameraSpace(_mouse.ScreenPosition);
            State?.Transform(worldMousePos);
        }

        public void StartTransform<T>()
            where T : IMouseControlledTransformSystemState, new()
        {
            var camera = EntityWorld.TagManager.GetEntity(CameraTag)?.GetComponent<CameraComponent>();
            var worldMousePosition = camera.ScreenToCameraSpace(_mouse.ScreenPosition);
            State = new T();
            State.Start(ActiveEntities, worldMousePosition);
        }

        public void CancelTransform()
        {
            State?.Cancel();
            State = null;
        }

        public void ApplyTransform()
        {
            State?.Apply();
        }
    }
}
