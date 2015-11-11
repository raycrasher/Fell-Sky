using Artemis.Interface;
using Artemis;
using FellSky.Graphics;

namespace FellSky.EntityTemplates
{
    [Artemis.Attributes.ArtemisEntityTemplate("Camera")]
    public class CameraEntityTemplate : IEntityTemplate
    {
        public Entity BuildEntity(Entity entity, EntityWorld entityWorld, params object[] args)
        {
            var camera = new Camera2D();
            entity.AddComponent(camera);
            entity.AddComponent(camera.Transform);
            return entity;
        }
    }
}
