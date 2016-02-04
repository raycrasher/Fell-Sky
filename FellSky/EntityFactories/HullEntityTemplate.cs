using Artemis;
using FellSky.Models.Ships.Parts;
using FellSky.Components;
using FellSky.Services;

namespace FellSky.EntityFactories
{
    public class HullEntityFactory
    {
        private ISpriteManagerService _spriteManager;

        public HullEntityFactory(ISpriteManagerService spriteManager)
        {
            _spriteManager = spriteManager;
        }

        public Entity BuildEntity(Entity ship, EntityWorld entityWorld, Hull hull, bool addPhysics = false)
        {
            var entity = entityWorld.CreateEntity();
            entity.AddComponent(new HullComponent(hull, ship, _spriteManager));
            entity.AddComponent(new LocalTransformComponent(ship));
            entity.AddComponent(hull.Transform);
            entity.AddComponent(new HealthComponent(hull.Health));

            if (addPhysics)
            {
                //entity.AddComponent(new PhysicsFixtureComponent());
            }
            return entity;
        }
    }
}
