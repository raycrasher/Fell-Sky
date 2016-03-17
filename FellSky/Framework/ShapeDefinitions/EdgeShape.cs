using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace FellSky.Framework.ShapeDefinitions
{
    public class EdgeShape : ShapeDefinition
    {
        public Vector2 Start { get; set; }
        public Vector2 End { get; set; }

        public override List<Fixture> Attach(Transform transform, Body body, object userdata = null)
            => new List<Fixture> { FarseerPhysics.Factories.FixtureFactory.AttachEdge(
                Vector2.Transform(Start, transform.Matrix), Vector2.Transform(End,transform.Matrix), body, userdata) };
    }
}
