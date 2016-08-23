using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FellSky.Components;
using FellSky.Services;
using FellSky.Systems;

namespace FellSky
{


    public class PhysicsCollisionEventArgs: EventArgs
    {
        public Fixture FixtureA, FixtureB;
        public Contact Contact;
        public Entity EntityA => FixtureA.UserData as Entity;
        public Entity EntityB => FixtureB.UserData as Entity;

        /// <summary>
        /// Valid only on AfterCollision events
        /// </summary>
        public ContactVelocityConstraint Impulse;
        public bool? IgnoreCollision = null;
    }

    public class PhysicsCollisionSystem: Artemis.System.EntitySystem
    {
        private IEventService _events;

        public World PhysicsWorld { get; private set; }
        public PhysicsCollisionSystem(IEventService eventService)
            : base(Aspect.All(typeof(CollisionComponent), typeof(RigidBodyFixtureComponent)))
        {
            _events = eventService;
        }

        public override void LoadContent()
        {
            PhysicsWorld = EntityWorld.SystemManager.GetSystem<PhysicsSystem>().PhysicsWorld;
            base.LoadContent();
        }

        public override void OnAdded(Entity entity)
        {
            var collision = entity.GetComponent<CollisionComponent>();
            var fixture = entity.GetComponent<RigidBodyFixtureComponent>();

            foreach(var item in fixture.Fixtures)
            {
                item.UserData = entity;
                if (collision.HandleCollision)
                    item.OnCollision += HandleCollision;
                if (collision.HandleBeforeCollision)
                    item.BeforeCollision += HandleBeforeCollision;
                if (collision.HandleAfterCollision)
                    item.AfterCollision += HandleAfterCollision;
                if (collision.HandleOnSeparation)
                    item.OnSeparation += HandleOnSeparation;
            }

            base.OnAdded(entity);
        }

        private void HandleOnSeparation(Fixture fixtureA, Fixture fixtureB)
        {
            var eventArgs = new PhysicsCollisionEventArgs
            {
                FixtureA = fixtureA,
                FixtureB = fixtureB
            };
            _events.FireEventNextFrame(this, EventId.PhysicsOnSeparation, eventArgs);
        }

        private void HandleAfterCollision(Fixture fixtureA, Fixture fixtureB, Contact contact, ContactVelocityConstraint impulse)
        {
            var eventArgs = new PhysicsCollisionEventArgs
            {
                FixtureA = fixtureA,
                FixtureB = fixtureB,
                Contact = contact,
                Impulse = impulse
            };
            _events.FireEventNextFrame(this, EventId.PhysicsOnSeparation, eventArgs);
        }

        private bool HandleBeforeCollision(Fixture fixtureA, Fixture fixtureB)
        {
            var eventArgs = new PhysicsCollisionEventArgs
            {
                FixtureA = fixtureA,
                FixtureB = fixtureB
            };
            _events.FireEventNextFrame(this, EventId.PhysicsOnSeparation, eventArgs);
            return eventArgs.IgnoreCollision ?? (fixtureA.CollidesWith | fixtureB.CollidesWith) > 0;
        }

        private bool HandleCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            var eventArgs = new PhysicsCollisionEventArgs
            {
                FixtureA = fixtureA,
                FixtureB = fixtureB,
                Contact = contact,
            };

            // filter here
            _events.FireEventNextFrame(this, EventId.PhysicsCollision, eventArgs);
            return eventArgs.IgnoreCollision ?? (fixtureA.CollidesWith | fixtureB.CollidesWith) > 0;
        }
    }
}
