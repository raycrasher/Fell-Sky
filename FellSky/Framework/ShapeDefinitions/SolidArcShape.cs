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

        public override List<Fixture> Attach(Transform transform, Body body, object userdata = null)
            => FarseerPhysics.Factories.FixtureFactory.AttachSolidArc(Density, Radians, Sides, Radius, Vector2.Transform(Position, transform.Matrix), Angle + transform.Rotation, body);
    }
}
