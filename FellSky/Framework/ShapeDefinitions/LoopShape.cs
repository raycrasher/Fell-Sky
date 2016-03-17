using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace FellSky.Framework.ShapeDefinitions
{
    class LoopShape : ShapeDefinition
    {
        public Vector2[] Vertices { get; set; }
        public override List<Fixture> Attach(Transform xform, Body body, object userdata = null) =>
            new List<Fixture> { FarseerPhysics.Factories.FixtureFactory.AttachLoopShape(TransformVertices(Vertices, xform), body, userdata) };
    }
}
