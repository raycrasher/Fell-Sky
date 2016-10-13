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
        PhysicsCollisionEventArgs _physicsCollisionArgs;

        public World PhysicsWorld { get; private set; }
        public PhysicsCollisionSystem()
            : base(Aspect.All(typeof(CollisionComponent), typeof(RigidBodyFixtureComponent)))
        {
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
            _physicsCollisionArgs.Contact = null;
            _physicsCollisionArgs.FixtureA = fixtureA;
            _physicsCollisionArgs.FixtureB = fixtureB;
            _physicsCollisionArgs.IgnoreCollision = false;

            EntityWorld.FireEvent(this, EventId.PhysicsOnSeparation, _physicsCollisionArgs);
            _physicsCollisionArgs.EntityA?.FireEvent(this, EventId.PhysicsOnSeparation, _physicsCollisionArgs);
            _physicsCollisionArgs.EntityB?.FireEvent(this, EventId.PhysicsOnSeparation, _physicsCollisionArgs);

        }

        private void HandleAfterCollision(Fixture fixtureA, Fixture fixtureB, Contact contact, ContactVelocityConstraint impulse)
        {
            _physicsCollisionArgs.FixtureA = fixtureA;
            _physicsCollisionArgs.FixtureB = fixtureB;
            _physicsCollisionArgs.Contact = contact;
            _physicsCollisionArgs.Impulse = impulse;
            _physicsCollisionArgs.IgnoreCollision = false;

            EntityWorld.FireEvent(this, EventId.AfterCollision, _physicsCollisionArgs);
            _physicsCollisionArgs.EntityA?.FireEvent(this, EventId.AfterCollision, _physicsCollisionArgs);
            _physicsCollisionArgs.EntityB?.FireEvent(this, EventId.AfterCollision, _physicsCollisionArgs);
        }

        private bool HandleBeforeCollision(Fixture fixtureA, Fixture fixtureB)
        {
            _physicsCollisionArgs.Contact = null;
            _physicsCollisionArgs.FixtureA = fixtureA;
            _physicsCollisionArgs.FixtureB = fixtureB;
            _physicsCollisionArgs.IgnoreCollision = false;
            
            EntityWorld.FireEvent(this, EventId.BeforeCollision, _physicsCollisionArgs);
            _physicsCollisionArgs.EntityA?.FireEvent(this, EventId.BeforeCollision, _physicsCollisionArgs);
            _physicsCollisionArgs.EntityB?.FireEvent(this, EventId.BeforeCollision, _physicsCollisionArgs);

            return _physicsCollisionArgs.IgnoreCollision ?? (fixtureA.CollidesWith | fixtureB.CollidesWith) > 0;
        }

        private bool HandleCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            _physicsCollisionArgs.Contact = contact;
            _physicsCollisionArgs.FixtureA = fixtureA;
            _physicsCollisionArgs.FixtureB = fixtureB;
            _physicsCollisionArgs.IgnoreCollision = false;

            // filter here
            EntityWorld.FireEvent(this, EventId.PhysicsCollision, _physicsCollisionArgs);
            _physicsCollisionArgs.EntityA?.FireEvent(this, EventId.PhysicsCollision, _physicsCollisionArgs);
            _physicsCollisionArgs.EntityB?.FireEvent(this, EventId.PhysicsCollision, _physicsCollisionArgs);


            return _physicsCollisionArgs.IgnoreCollision ?? (fixtureA.CollidesWith | fixtureB.CollidesWith) > 0;
        }
    }
}
