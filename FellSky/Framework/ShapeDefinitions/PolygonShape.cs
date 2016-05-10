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
        public float Density { get; set; } = 1;
        public Vector2[] Vertices { get; set; }

        public override List<Fixture> Attach(ref Matrix xform, Body body, object userdata = null)
        {
            Vector2 pos, scale;
            float rot;
            Utilities.DecomposeMatrix2D(ref xform, out pos, out rot, out scale);
            var vertices = TransformVertices(Vertices, ref xform);

            var flip = scale.X < 0 ^ scale.Y < 0;
            if (flip) vertices.Reverse();
            vertices.ForceCounterClockWise();
            return new List<Fixture> { FarseerPhysics.Factories.FixtureFactory.AttachPolygon(
                vertices, Density, body, userdata) };
        }
        
    }
}
