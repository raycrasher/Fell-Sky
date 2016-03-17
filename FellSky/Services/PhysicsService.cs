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

namespace FellSky.Services
{
    public interface IPhysicsService
    {
        World World { get; set; }
        RigidBodyComponent CreateRigidBody(Vector2 position, float rotation);
        RigidBodyFixtureComponent CreateAndAttachFixture(RigidBodyComponent component, string shapeId, Transform transform);
        void LoadShapes(string filename);
        void SaveShapes(string filename);
        void AddShape(ShapeDefinition shape);
    }

    public class PhysicsService : IPhysicsService
    {
        private Dictionary<string, ShapeDefinition> _shapes = new Dictionary<string, ShapeDefinition>();
        public World World { get; set; }

        public PhysicsService()
        {
            World = new World(Vector2.Zero);
        }

        public void LoadShapes(string filename)
        {
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
                Body = FarseerPhysics.Factories.BodyFactory.CreateBody(World)
            };
        }

        public void AddShape(ShapeDefinition shape)
        {
            _shapes[shape.Id] = shape;
        }
    }


}
