using System.Collections.Generic;
using System.Linq;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Common;
using System;

namespace FellSky.Framework.ShapeDefinitions
{
    public class CompoundPolygonShape : ShapeDefinition
    {
        public float Density { get; private set; }
        public List<Vector2[]> Polygons { get; set; }

        public override List<Fixture> Attach(ref Matrix xform, Body body, object userdata = null)
        {
            var transform = xform;
            return FarseerPhysics.Factories.FixtureFactory.AttachCompoundPolygon(
                Polygons.Select(p => TransformVertices(p, ref transform)).ToList(), Density, body, userdata);
        }
        
        
    }
}
