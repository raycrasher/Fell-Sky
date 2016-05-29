using Artemis;
using FellSky.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FellSky
{
    public static class UtilityExtensions
    {
        public static Matrix GetMatrix(this ITransform xform)
        {
            return Matrix.CreateTranslation(new Vector3(-xform.Origin,0)) * 
                   Matrix.CreateScale(new Vector3(xform.Scale, 1)) * 
                   Matrix.CreateRotationZ(xform.Rotation) * 
                   Matrix.CreateTranslation(new Vector3(xform.Position, 0));
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

        public static T GetService<T>(this IServiceProvider provider)
        {
            return (T)provider.GetService(typeof(T));
        }
        /*
        public static T GetService<T>(this Artemis.Blackboard.BlackBoard blackboard)
            where T : class
        {
            return blackboard.GetEntry<IServiceProvider>("ServiceProvider")?.GetService<T>();
        }
        */

        public static Camera GetActiveCamera(this EntityWorld world)
        {
            var camera = world.TagManager.GetEntity(Constants.ActiveCameraTag)?.GetComponent<Camera>();
            if (camera == null)
                throw new InvalidOperationException("There is no active camera.");
            return camera;
        }

        public static Matrix GetWorldMatrix(this Artemis.Entity entity)
        {
            var xform = entity.GetComponent<Transform>();
            var matrix = xform.GetMatrix();
            var child = entity.GetComponent<LocalTransformComponent>();
            if (child != null)
                matrix = child.ParentWorldMatrix * matrix;
            return matrix;
        }

        public static ICollection<T> AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
                collection.Add(item);
            return collection;
        }

        public static float NextFloat(this System.Random rng, float min, float max)
        {
            return min + ((float)rng.NextDouble() * (max - min));
        }

        public static void Move<T>(this IList<T> list, int oldIndex, int newIndex)
        {
            var item = list[oldIndex];
            list.RemoveAt(oldIndex);
            list.Insert(newIndex, item);
        }

        public static Vector2 GetPerpendicularLeft(this Vector2 v) => new Vector2(v.Y, -v.X);
        public static Vector2 GetPerpendicularRight(this Vector2 v) => new Vector2(-v.Y, v.X);
        public static Vector2 ToUnitVector(this Vector2 v) => v / v.Length();

        public static float GetDeterminant(this Vector2 vector, Vector2 point)
        {
            return vector.X * point.Y - vector.Y * point.X;
        }

        public static Color NextRandomColor(this Random rng)
            => new Color(rng.NextFloat(0, 1), rng.NextFloat(0, 1), rng.NextFloat(0, 1));
    }
}
