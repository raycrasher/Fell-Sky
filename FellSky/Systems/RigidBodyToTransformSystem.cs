using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using FellSky.Components;

namespace FellSky.Systems
{
    class RigidBodyToTransformSystem: Artemis.System.ParallelEntityProcessingSystem
    {
        public RigidBodyToTransformSystem()
            : base(Aspect.All(typeof(Transform), typeof(RigidBodyComponent)))
        {
        }

        public override void Process(Entity entity)
        {
            var transform = entity.GetComponent<Transform>();
            var rigidbody = entity.GetComponent<RigidBodyComponent>();
            transform.Position = rigidbody.Body.Position;
            transform.Rotation = rigidbody.Body.Rotation;
        }
    }
}
