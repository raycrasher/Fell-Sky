using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace FellSky.Framework.ShapeDefinitions
{
    public class EdgeShape : ShapeDefinition
    {
        public Vector2 Start { get; set; }
        public Vector2 End { get; set; }

        public override List<Fixture> Attach(ref Matrix xform, Body body, object userdata = null)
        {
            return new List<Fixture> { FarseerPhysics.Factories.FixtureFactory.AttachEdge(
                Vector2.Transform(Start, xform), Vector2.Transform(End, xform), body, userdata) };
        }
    }
}
