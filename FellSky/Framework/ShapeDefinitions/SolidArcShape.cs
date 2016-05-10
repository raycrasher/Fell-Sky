using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace FellSky.Framework.ShapeDefinitions
{
    public class SolidArcShape : ShapeDefinition
    {
        public float Density { get; set; }
        public float Radians { get; set; }
        public float Radius { get; set; }
        public int Sides { get; set; }
        public Vector2 Position { get; set; }
        public float Angle { get; set; }

        public override List<Fixture> Attach(ref Matrix xform, Body body, object userdata = null)
        {
            Vector2 pos, scale;
            float rot;
            Utilities.DecomposeMatrix2D(ref xform, out pos, out rot, out scale);

            return FarseerPhysics.Factories.FixtureFactory.AttachSolidArc(Density, Radians, Sides, Radius, Vector2.Transform(Position, xform), Angle + rot, body);
        }
    }
}
