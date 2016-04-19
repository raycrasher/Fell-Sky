using Artemis.Interface;
using Microsoft.Xna.Framework;

namespace FellSky.Game.Space
{
    public sealed class SpaceObject: IComponent
    {
        public Vector2 Position { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string SpriteId { get; set; }
        public string IconSpriteId { get; set; }
    }
}
