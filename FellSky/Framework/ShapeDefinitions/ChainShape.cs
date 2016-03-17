using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace FellSky.Framework.ShapeDefinitions
{
    public class ChainShape : ShapeDefinition
    {
        public Vector2[] Vertices { get; set; }
        public override List<Fixture> Attach(Transform transform, Body body, object userdata = null) => 
            new List<Fixture> {
                FarseerPhysics.Factories.FixtureFactory.AttachChainShape(
                    TransformVertices(Vertices, transform), body, userdata) };
    }
}
