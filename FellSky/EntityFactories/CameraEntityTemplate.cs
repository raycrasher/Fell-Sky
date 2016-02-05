using Artemis.Interface;
using Artemis;

using FellSky.Components;

namespace FellSky.EntityFactories
{
    public class CameraEntityFactory
    {
        public CameraEntityFactory(EntityWorld world)
        {
            World = world;
        }

        public EntityWorld World { get; private set; }

        public Entity CreateCamera()
        {
            var entity = World.CreateEntity();
            var camera = new CameraComponent();
            entity.AddComponent(camera);
            entity.AddComponent(camera.Transform);
            return entity;
        }
    }
}
