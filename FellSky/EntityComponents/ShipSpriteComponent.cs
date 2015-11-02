using Artemis.Interface;
using FellSky.Graphics;
using FellSky.Mechanics.Ships;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.EntityComponents
{
    public delegate void ShipSpriteDrawFunction(SpriteBatch batch);

    public class ShipSpriteComponent: IComponent
    {
        public ShipSpriteComponent(Ship ship)
        {
            Ship = ship;
        }
        public Ship Ship { get; set; }
    }   
}
