using FellSky.Common;
using Microsoft.Xna.Framework;

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
    }
}
