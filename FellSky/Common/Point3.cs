using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Common
{
    public struct Point3
    {
        public int X, Y, Z;

        public Point3(int x, int y, int z)
        {
            X = x; Y = y; Z = z;
        }

        public Point3(Point p, int z=0)
        {
            X = p.X; Y = p.Y; Z = z;
        }

        public static Point3 Zero { get; } = new Point3(0, 0, 0);

        public static Point3 operator +(Point3 a, Point3 b)
        {
            return new Point3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Point3 operator -(Point3 a, Point3 b)
        {
            return new Point3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Point3 operator *(Point3 a, Point3 b)
        {
            return new Point3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        public static Point3 operator /(Point3 a, Point3 b)
        {
            return new Point3(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }

        public static Point3 operator +(Point3 a, Point b)
        {
            return new Point3(a.X + b.X, a.Y + b.Y, a.Z);
        }

        public static Point3 operator -(Point3 a, Point b)
        {
            return new Point3(a.X - b.X, a.Y - b.Y, a.Z);
        }

        public static Point3 operator *(Point3 a, Point b)
        {
            return new Point3(a.X * b.X, a.Y * b.Y, a.Z);
        }

        public static Point3 operator /(Point3 a, Point b)
        {
            return new Point3(a.X / b.X, a.Y / b.Y, a.Z);
        }

        public static implicit operator Point3(Point p)
        {
            return new Point3(p);
        }

        public static implicit operator Point(Point3 p)
        {
            return new Point(p.X,p.Y);
        }
    }
}
