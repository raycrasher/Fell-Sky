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
using FellSky.Framework.ShapeDefinitions;
using Newtonsoft.Json.Linq;
using Artemis;
using Newtonsoft.Json;
using FellSky.Services;

namespace FellSky.Systems
{
    public class PhysicsSystem: Artemis.System.ProcessingSystem
    {
        private ITimerService _timer;
        private IShapeManagerService _shapeManager;

        public World PhysicsWorld { get; set; }
        public float UnitScale { get; set; }

        public PhysicsSystem(ITimerService timer, IShapeManagerService shapeManager)
        {
            _shapeManager = shapeManager;
            PhysicsWorld = new World(Vector2.Zero);
            _timer = timer;
        }


        public RigidBodyFixtureComponent CreateAndAttachFixture(RigidBodyComponent bodyComponent, string shapeId, Transform transform)
        {
            var fixtures = _shapeManager.GetShape(shapeId).Attach(transform, bodyComponent.Body);
            var fixtureComponent = new RigidBodyFixtureComponent
            {
                Fixtures = fixtures
            };
            return fixtureComponent;
        }

        public RigidBodyComponent CreateRigidBody(Vector2 position, float rotation)
        {
            return new RigidBodyComponent
            {
                Body = FarseerPhysics.Factories.BodyFactory.CreateBody(PhysicsWorld)
            };
        }

        public override void ProcessSystem()
        {
            PhysicsWorld.Step((float)_timer.DeltaTime.TotalSeconds);
        }
    }


}
