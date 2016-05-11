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
        const float UnitScale = Constants.PhysicsUnitScale;

        public PhysicsSystem()
        {
            _shapeManager = ServiceLocator.Instance.GetService<IShapeManagerService>();
            PhysicsWorld = new World(Vector2.Zero);
            _timer = ServiceLocator.Instance.GetService<ITimerService>();
            //FarseerPhysics.ConvertUnits.SetDisplayUnitToSimUnitRatio(1f / UnitScale);
        }


        public RigidBodyFixtureComponent CreateAndAttachFixture(RigidBodyComponent bodyComponent, string shapeId, Matrix matrix)
        {
            var fixtures = _shapeManager.GetShape(shapeId).Attach(ref matrix, bodyComponent.Body);
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
                Body = FarseerPhysics.Factories.BodyFactory.CreateBody(PhysicsWorld, position * UnitScale, rotation)
            };
        }

        public override void ProcessSystem()
        {
            PhysicsWorld.Step((float)_timer.DeltaTime.TotalSeconds);
        } 
    }


}
