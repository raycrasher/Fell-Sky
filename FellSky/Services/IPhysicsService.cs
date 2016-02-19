using FarseerPhysics.Collision.Shapes;
using FellSky.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis.Interface;
using FarseerPhysics.Dynamics;

namespace FellSky.Services
{
    public interface IPhysicsService
    {
        RigidBodyComponent CreateRigidBody(Vector2 position, float rotation);
        RigidBodyFixtureComponent CreateAndAttachFixture(RigidBodyComponent body, string shapeId, Transform transform);
    }
}
