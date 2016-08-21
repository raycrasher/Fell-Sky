using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using FarseerPhysics.Dynamics;
using FellSky.Components;

namespace FellSky.Systems
{
    class RigidBodySystem: Artemis.System.ParallelEntityProcessingSystem
    {
        private World _physicsWorld;

        public RigidBodySystem()
            : base(Aspect.All(typeof(RigidBodyComponent)))
        {
        }

        public override void LoadContent()
        {
            EntityWorld.EntityManager.RemovedEntityEvent += OnRemove;
            _physicsWorld = EntityWorld.SystemManager.GetSystem<PhysicsSystem>().PhysicsWorld;
        }

        public override void Process(Entity entity)
        {
            var transform = entity.GetComponent<Transform>();
            var rigidbody = entity.GetComponent<RigidBodyComponent>();
            if (transform != null)
            {
                transform.Position = rigidbody.Body.Position / Constants.PhysicsUnitScale;
                transform.Rotation = rigidbody.Body.Rotation;
            }
        }

        public override void OnAdded(Entity entity)
        {
            base.OnAdded(entity);
            
        }

        public void OnRemove(Entity entity)
        {
            var component = entity.GetComponent<RigidBodyComponent>();
            if(component != null)
                _physicsWorld.RemoveBody(component.Body);
        }
    }
}
