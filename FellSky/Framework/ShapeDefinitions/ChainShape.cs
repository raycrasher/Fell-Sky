using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System;

namespace FellSky.Framework.ShapeDefinitions
{
    public class ChainShape : ShapeDefinition
    {
        public Vector2[] Vertices { get; set; }

        public override List<Fixture> Attach(ref Matrix xform, Body body, object userdata = null)
        {
            return new List<Fixture>
            {
                FarseerPhysics.Factories.FixtureFactory.AttachChainShape(TransformVertices(Vertices, ref xform), body, userdata)
            };
        }
    }
}
