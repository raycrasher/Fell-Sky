using Artemis.Interface;
using Artemis;
using Microsoft.Xna.Framework;
using FellSky.Components;
using FellSky.Framework;

namespace FellSky.EntityFactories
{
    [Artemis.Attributes.ArtemisEntityTemplate("GenericDrawable")]
    public class GenericDrawableTemplate : IEntityTemplate
    {
        public Entity BuildEntity(Entity entity, EntityWorld entityWorld, params object[] args)
        {
            entity.AddComponent(new GenericDrawableComponent { DrawFunction = (GenericDrawableComponent.DrawDelegate)args[0] });
            return entity;
        }

        public static GenericDrawableComponent.DrawDelegate Circle(Vector2 center, float radius, int sides, Color color)
            => (a, b, e) => b.DrawCircle(center, radius, sides, color);

        public static GenericDrawableComponent.DrawDelegate Line(Vector2 point, float length, int angle, Color color)
            => (a, b, e) => b.DrawLine(point, length, angle, color);
    }
}
