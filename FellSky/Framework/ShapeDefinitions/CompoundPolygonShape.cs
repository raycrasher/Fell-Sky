using System.Collections.Generic;
using System.Linq;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Common;

namespace FellSky.Framework.ShapeDefinitions
{
    public class CompoundPolygonShape : ShapeDefinition
    {
        public float Density { get; private set; }
        public List<Vector2[]> Polygons { get; set; }
        public override List<Fixture> Attach(Transform transform, Body body, object userdata = null)
            => FarseerPhysics.Factories.FixtureFactory.AttachCompoundPolygon(
                Polygons.Select(p => TransformVertices(p, transform)).ToList(), Density, body, userdata);
        
    }
}
