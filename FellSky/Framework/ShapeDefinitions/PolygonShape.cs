using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Common;

namespace FellSky.Framework.ShapeDefinitions
{
    public class PolygonShape : ShapeDefinition
    {
        public float Density { get; private set; }
        public Vector2[] Vertices { get; set; }
        public override List<Fixture> Attach(Transform xform, Body body, object userdata = null)
            => new List<Fixture> { FarseerPhysics.Factories.FixtureFactory.AttachPolygon(
                TransformVertices(Vertices, xform), Density, body, userdata) };
    }
}
