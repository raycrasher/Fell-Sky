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
        private Dictionary<string, ShapeDefinition> _shapes = new Dictionary<string, ShapeDefinition>();
        private ITimerService _timer;

        public World PhysicsWorld { get; set; }
        public float UnitScale { get; set; }

        public PhysicsSystem(ITimerService timer)
        {
            PhysicsWorld = new World(Vector2.Zero);
            _timer = timer;
        }

        public void LoadShapes(string filename)
        {
            if(System.IO.File.Exists(filename))
                _shapes = JsonConvert.DeserializeObject<Dictionary<string, ShapeDefinition>>(System.IO.File.ReadAllText(filename));
        }

        public void SaveShapes(string filename)
        {
            System.IO.File.WriteAllText(filename, JsonConvert.SerializeObject(_shapes));
        }

        public RigidBodyFixtureComponent CreateAndAttachFixture(RigidBodyComponent bodyComponent, string shapeId, Transform transform)
        {
            var fixtures = _shapes[shapeId].Attach(transform, bodyComponent.Body);
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

        public void AddShape(ShapeDefinition shape)
        {
            _shapes[shape.Id] = shape;
        }

        public override void ProcessSystem()
        {
            PhysicsWorld.Step((float)_timer.DeltaTime.TotalSeconds);
        }
    }


}
