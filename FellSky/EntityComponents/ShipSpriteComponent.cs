using Artemis.Interface;
using FellSky.Mechanics.Ships;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.EntityComponents
{
    public class ShipSpriteComponent: IComponent
    {
        public ShipSpriteComponent(Ship ship)
        {
            Ship = ship;
        }

        public Ship Ship { get; set; }

        public List<HullGlowItem> GlowItems { get; } = new List<HullGlowItem>();

        public void AddHullGlow(Hull hull, Color color, TimeSpan glowTime)
        {
            GlowItems.Add(new HullGlowItem { Hull = hull, Color = color, Age = TimeSpan.Zero, MaxAge = glowTime });
        }        

        public struct HullGlowItem
        {
            public Hull Hull;
            public Color Color;
            public TimeSpan Age, MaxAge;
        }
    }
}
