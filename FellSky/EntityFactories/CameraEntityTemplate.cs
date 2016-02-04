using Artemis.Interface;
using Artemis;

using FellSky.Components;

namespace FellSky.EntityFactories
{
    [Artemis.Attributes.ArtemisEntityTemplate("Camera")]
    public class CameraEntityTemplate : IEntityTemplate
    {
        public Entity BuildEntity(Entity entity, EntityWorld entityWorld, params object[] args)
        {
            var camera = new CameraComponent();
            entity.AddComponent(camera);
            entity.AddComponent(camera.Transform);
            return entity;
        }
    }
}
