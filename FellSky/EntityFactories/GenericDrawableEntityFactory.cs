using Artemis.Interface;
using Artemis;
using Microsoft.Xna.Framework;
using FellSky.Components;
using FellSky.Framework;

namespace FellSky.EntityFactories
{
    public static class GenericDrawableEntityFactory
    {
        public static Entity CreateCircle(this EntityWorld world, Vector2 center, float radius, int sides, Color color)
        {
            var entity = world.CreateEntity();
            entity.AddComponent(new GenericDrawableComponent((a, b, e) => b.DrawCircle(center, radius, sides, color)));
            return entity;
        }

        public static Entity CreateLine(this EntityWorld world, Vector2 point, float length, int angle, Color color)
        {
            var entity = world.CreateEntity();
            entity.AddComponent(new GenericDrawableComponent((a, b, e) => b.DrawLine(point, length, angle, color)));
            return entity;
        }
    }
}
