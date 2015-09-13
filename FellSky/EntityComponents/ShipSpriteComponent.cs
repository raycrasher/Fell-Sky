using Artemis.Interface;
using FellSky.Common;
using FellSky.Graphics;
using FellSky.Mechanics.Ships;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace FellSky.EntityComponents
{
    public class ShipSprite
    {
        public Sprite Sprite { get; set; }
        public ITransform Transform { get; set; }
        public Color Color { get; set; }
    }

    public class ShipSpriteComponent: IComponent
    {

        
        public Ship Ship { get; set; }
        public List<ShipSprite> Sprites { get; set; }
        public List<ShipSprite> Overlays { get; set; }

        private void LoadSpritesFromShip()
        {
            //Sprites = Ship.Hulls.Select(s=>s.SpriteId)
        }
    }
}
