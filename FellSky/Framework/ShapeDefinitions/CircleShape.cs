using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace FellSky.Framework.ShapeDefinitions
{
    public class CircleShape : ShapeDefinition
    {
        public float Density { get; set; }
        public Vector2 Offset { get; set; }
        public float Radius { get; set; }

        public override List<Fixture> Attach(ref Matrix xform, Body body, object userdata = null)
        {
            return new List<Fixture> { FarseerPhysics.Factories.FixtureFactory.AttachCircle(
                Radius, Density, body, Vector2.Transform(Offset, xform), userdata) };
        }
    }
}
