using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace FellSky.Framework.ShapeDefinitions
{
    public class EllipseShape : ShapeDefinition
    {
        public float Density { get; set; }
        public int NumEdges { get; set; }
        public Vector2 Radius { get; set; }

        public override List<Fixture> Attach(ref Matrix xform, Body body, object userdata = null)
        {
            return new List<Fixture> { FarseerPhysics.Factories.FixtureFactory.AttachEllipse(Radius.X, Radius.Y, NumEdges, Density, body, userdata) };
        }

        
    }
}
