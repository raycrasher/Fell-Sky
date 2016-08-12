using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace FellSky.Game.Space
{
    public class SpaceObject
    {
        public SpaceObject() { }
        public SpaceObject(params SpaceObject[] children)
        {
            Children.AddRange(children);
        }

        public Vector2 Position { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string SpriteId { get; set; }
        public string IconSpriteId { get; set; }

        public float Mass { get; set; }

        public List<SpaceObject> Children { get; set; } = new List<SpaceObject>();
    }
}
