using Artemis.Interface;
using FellSky.Common;
using FellSky.Graphics;
using FellSky.Mechanics.Ships;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace FellSky.EntityComponents
{
    public class ShipSpriteComponent: IComponent
    {
        public class SpriteRecord
        {
            public Sprite Sprite { get; set; }
            public ITransform Transform { get; set; }
            public float HeatGlowOpacity { get; set; }
            public float DamageTextureOpacity { get; set; }
            public Rectangle RenderClipSize { get; set; }
        }
        
        public Ship Ship { get; set; }
        public List<SpriteRecord> SpriteEntries { get; set; }

        private void LoadSpritesFromShip()
        {
            //Sprites = Ship.Hulls.Select(s=>s.SpriteId)
        }
    }
}
