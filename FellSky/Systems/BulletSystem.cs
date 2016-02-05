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
        private DiplomacySystem _diplomacy;
        private IEventService _events;

        public List<BulletHitEventArgs> BulletHitEvents { get; } = new List<BulletHitEventArgs>();

        public BulletSystem(IEventService events)
        {
            _events = events;
        }

        public override void LoadContent()
        {
            _events.AddEventListener(EventId.PhysicsCollision, CollisionEventHandler);
            _diplomacy = EntityWorld.SystemManager.GetSystem<DiplomacySystem>();
            base.LoadContent();
        }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            foreach(var item in BulletHitEvents)
            {
                var damage = item.Bullet.GetComponent<DamageComponent>();
                var health = item.Bullet.GetComponent<HealthComponent>();

                if (health != null && damage!=null)
                {
                    health.ApplyDamage(damage);
                }

                _events.FireEvent(this, EventId.BulletHit, item);
            }
            BulletHitEvents.Clear();
            base.ProcessEntities(entities);
        }

        private void CollisionEventHandler(object sender, EventArgs e)
        {
            var args = (PhysicsCollisionEventArgs) e;
            var bullet = args.EntityA?.GetComponent<BulletComponent>();
            if (bullet == null) return;

            if(!bullet.IsIndiscriminate)
            {
                var factionA = args.EntityA?.GetComponent<FactionComponent>();
                var factionB = args.EntityB?.GetComponent<FactionComponent>();
                if (!_diplomacy.IsFriendlyFirePossibleBetween(factionA, factionB))
                    args.IgnoreCollision = true;
            }

            BulletHitEvents.Add(new BulletHitEventArgs
            {
                Bullet = args.EntityA,
                Target = args.EntityB
            });
        }

        public override void UnloadContent()
        {
            _events.RemoveEventListener(EventId.PhysicsCollision, CollisionEventHandler);
            base.UnloadContent();
        }
    }
}
