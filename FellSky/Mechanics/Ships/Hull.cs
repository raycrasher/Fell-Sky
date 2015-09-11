using FellSky.Common;
using Microsoft.Xna.Framework;
using System;

namespace FellSky.Mechanics.Ships
{
    public enum HullOrientation
    {
        Up, Right, Down, Left
    }

    public class Hull
    {
        public float MaxHealth { get; set; }
        public float Health { get; set; }
        public string SpriteId { get; set; }
        public string ShapeId { get; set; }
        public Color Color { get; set; }
        public HullOrientation Orientation { get; set; }
        public Point Position { get; set; }
        public Point Origin { get; set; }
        public Point? Size
        {
            get {
                if (StructureMap == null) return null;
                return new Point(StructureMap.GetLength(0), StructureMap.GetLength(1));
            }
        }
        public int? Height
        {
            get {
                if (StructureMap == null) return null;
                return StructureMap.GetLength(2);
            }
        }

        public int[,,] StructureMap { get; set; }
    }
}
