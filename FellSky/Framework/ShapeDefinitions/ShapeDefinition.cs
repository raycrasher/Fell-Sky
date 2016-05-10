using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FellSky.Framework.ShapeDefinitions
{
    public abstract class ShapeDefinition
    {
        public string Id { get; set; }
        public abstract List<Fixture> Attach(ref Matrix xform, Body body, object userdata = null);

        protected static Vertices TransformVertices (Vector2[] vertices, ref Matrix xform)
        {
            Vector2[] dest = new Vector2[vertices.Length];
            Vector2.Transform(vertices, ref xform, dest);
            return new Vertices(dest);
        }
    }


}
