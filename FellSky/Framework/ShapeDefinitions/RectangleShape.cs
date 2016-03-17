using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace FellSky.Framework.ShapeDefinitions
{
    public class RectangleShape : ShapeDefinition
    {
        public float Density { get; set; }
        public float Height { get; set; }
        public Vector2 Offset { get; set; }
        public float Width { get; set; }

        public override List<Fixture> Attach(Transform xform, Body body, object userdata = null)
            => new List<Fixture> { FarseerPhysics.Factories.FixtureFactory.AttachRectangle(Width, Height, Density, Vector2.Transform(Offset, xform.Matrix), body, userdata) };
    }
}
