using Artemis;
using FellSky.Components;
using FellSky.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Systems
{
    public static partial class EventId
    {
        public static int BulletHit;
    }

    public class BulletHitEventArgs : EventArgs
    {
        public Entity Bullet;
        public Entity Target;
    }

    public class BulletSystem: Artemis.System.EntitySystem
    {
        BulletHitEventArgs hitArgs = new BulletHitEventArgs();

        public BulletSystem()
            : base(Aspect.All(typeof(BulletComponent)))
        {
        }

        public override void LoadContent()
        {
            EntityWorld.RegisterListener(EventId.PhysicsCollision, CollisionEventHandler);
        }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            foreach(var entity in entities.Values)
            {
                var bulletComponnet = entity.GetComponent<BulletComponent>();
                bulletComponnet.Age += TimeSpan.FromMilliseconds(EntityWorld.Delta);
                if (bulletComponnet.Age >= bulletComponnet.Bullet.MaxAge)
                {
                    entity.Delete();
                }
            }
        }

        private void CollisionEventHandler(object sender, EventArgs e)
        {
            var args = (PhysicsCollisionEventArgs) e;
            var bullet = args.EntityA?.GetComponent<Components.BulletComponent>();
            if (bullet == null) return;
            /*
            if(!bullet.IsIndiscriminate)
            {
                var factionA = args.EntityA?.GetComponent<FactionComponent>();
                var factionB = args.EntityB?.GetComponent<FactionComponent>();
                if (!_diplomacy.IsFriendlyFirePossibleBetween(factionA, factionB))
                    args.IgnoreCollision = true;
            }
            */
            hitArgs.Bullet = args.EntityA;
            hitArgs.Target = args.EntityB;

            args.EntityB.FireEvent(this, EventId.BulletHit, hitArgs);
            args.EntityA.FireEvent(this, EventId.BulletHit, hitArgs);
            EntityWorld.FireEvent(this, EventId.BulletHit, hitArgs);
        }

        public override void UnloadContent()
        {
            EntityWorld.UnregisterListener(EventId.PhysicsCollision, CollisionEventHandler);
            base.UnloadContent();
        }
    }
}
