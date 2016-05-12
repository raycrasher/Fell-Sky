using Artemis.Interface;
using Artemis;

using FellSky.Components;
using Microsoft.Xna.Framework.Graphics;

namespace FellSky.EntityFactories
{
    public static class CameraEntityFactory
    {
        public static Entity CreateCamera(this EntityWorld world, string tag, GraphicsDevice device)
        {
            var entity = world.CreateEntity();
            var camera = new Camera(device);
            entity.AddComponent(camera);
            entity.AddComponent(camera.Transform);
            entity.Tag = tag;
            return entity;
        }
    }
}
