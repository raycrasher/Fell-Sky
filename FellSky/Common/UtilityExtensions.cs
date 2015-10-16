using FellSky.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky
{


    public static class UtilityExtensions
    {
        public static Matrix GetMatrix(this ITransform xform)
        {
            return Matrix.CreateTranslation(new Vector3(-xform.Origin,0)) * Matrix.CreateScale(new Vector3(xform.Scale, 1)) * Matrix.CreateRotationZ(xform.Rotation) * Matrix.CreateTranslation(new Vector3(xform.Position, 0));
        }

        public static Matrix GetMatrixNoOrigin(this ITransform xform)
        {
            return Matrix.CreateScale(new Vector3(xform.Scale, 1)) * Matrix.CreateRotationZ(xform.Rotation) * Matrix.CreateTranslation(new Vector3(xform.Position, 0));
        }

        public static Color ToColorFromHexString(this string str)
        {
            var color = new Color();
            color.PackedValue = uint.Parse(str, System.Globalization.NumberStyles.HexNumber);
            return color;
        }

        public static Vector2 ToVector2(this string str)
        {
            var items = str.Split(',').Select(i => float.Parse(i)).ToArray();
            return new Vector2(items[0],items[1]);
        }

        public static ColorHSL ToHSL(this Color color)
        {
            return new ColorHSL(color);
        }

        public static Vector2 ToVector(this float angle)
        {
            return new Vector2((float)Math.Sin(angle), -(float)Math.Cos(angle));
        }

        public static float ToAngleRadians(this Vector2 vector)
        {
            return (float)Math.Atan2(vector.X, -vector.Y);
        }

        public static float DegreeToRadian(this float angle)
        {
            return (float) (Math.PI * angle / 180.0);
        }

        public static float RadianToDegree(this float angle)
        {
            return (float)(angle * (180.0 / Math.PI));
        }

        public static float GetLesserAngleDifference(this float a1, float a2)
        {
            return (float) Math.Atan2(Math.Sin(a1 - a2), Math.Cos(a1 - a2));
        }
    }
}
