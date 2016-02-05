using Artemis.Interface;
using Artemis;
using Microsoft.Xna.Framework;
using FellSky.Components;
using FellSky.Framework;

namespace FellSky.EntityFactories
{
    public class GenericDrawableFactory
    {
        public GenericDrawableFactory(EntityWorld world)
        {
            World = world;
        }

        public EntityWorld World { get; private set; }

        public Entity CreateCircle(Vector2 center, float radius, int sides, Color color)
        {
            var entity = World.CreateEntity();
            entity.AddComponent(new GenericDrawableComponent((a, b, e) => b.DrawCircle(center, radius, sides, color)));
            return entity;
        }

        public Entity CreateLine(Vector2 point, float length, int angle, Color color)
        {
            var entity = World.CreateEntity();
            entity.AddComponent(new GenericDrawableComponent((a, b, e) => b.DrawLine(point, length, angle, color)));
            return entity;
        }
    }
}
