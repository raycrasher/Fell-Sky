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
            _events.FireEventNextFrame(this, EventId.BulletHit, new BulletHitEventArgs
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
