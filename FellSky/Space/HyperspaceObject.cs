using Artemis.Interface;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Space
{
    public class HyperspaceObject: IComponent
    {
        public Vector2 HyperspacePosition { get; set; }
        public SpaceObject RealSpaceObject { get; set; }
        public string Name { get; set; }
        public string SpriteId { get; set; }
        public string IconSpriteId { get; set; }
    }
}
