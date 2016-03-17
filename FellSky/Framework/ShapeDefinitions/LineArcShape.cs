using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace FellSky.Framework.ShapeDefinitions
{
    public class LineArcShape : ShapeDefinition
    {
        public float Angle { get; set; }
        public bool IsClosed { get; set; }
        public Vector2 Position { get; set; }
        public float Radians { get; set; }
        public float Radius { get; set; }
        public int Sides { get; set; }

        public override List<Fixture> Attach(Transform xform, Body body, object userdata = null)
        => new List<Fixture> { FarseerPhysics.Factories.FixtureFactory.AttachLineArc(
            Radians, Sides, Radius, Vector2.Transform(Position, xform.Matrix), Angle, IsClosed, body) };
    }
}
