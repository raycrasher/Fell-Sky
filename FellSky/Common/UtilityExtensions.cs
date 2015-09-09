using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Common
{
    public static class UtilityExtensions
    {
        public static Matrix GetMatrix(this ITransform xform)
        {
            return Matrix.CreateScale(new Vector3(xform.Scale, 1)) * Matrix.CreateRotationZ(xform.Rotation) * Matrix.CreateTranslation(new Vector3(xform.Position, 0));
        }
    }
}
