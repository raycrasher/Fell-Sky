using Artemis;
using DiceNotation.Rollers;
using FellSky.Components;
using FellSky.Services;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky
{


    public class BulletHitEventArgs : EventArgs
    {
        public Entity Bullet;
        public Entity Target;
        public float Damage;
        public Vector2 Position;
    }

    public class BulletSystem: Artemis.System.EntitySystem
    {
        BulletHitEventArgs hitArgs = new BulletHitEventArgs();
        IDieRoller _dieRoller = new StandardDieRoller(new Random());

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
                var bulletComponent = entity.GetComponent<BulletComponent>();
                bulletComponent.Age += TimeSpan.FromMilliseconds(EntityWorld.Delta);
                if (bulletComponent.Age >= bulletComponent.Bullet.MaxAge)
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
            hitArgs.Damage = bullet.Damage.Roll(_dieRoller).Value;

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
