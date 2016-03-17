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
        public abstract List<Fixture> Attach(Transform xform, Body body, object userdata = null);

        protected static Vertices TransformVertices (Vector2[] vertices, Transform xform)
        {
            var matrix = xform.Matrix;
            Vector2[] dest = new Vector2[vertices.Length];
            Vector2.Transform(vertices, ref matrix, dest);
            return new Vertices(dest);
        }
    }


}
