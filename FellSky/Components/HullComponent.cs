using System;
using Artemis;
using Artemis.Interface;
using FellSky.Models.Ships.Parts;
using FellSky.Services;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace FellSky.Components
{
    public class HullComponent: IComponent, IPartComponent<Hull>
    {
        public HullComponent(Hull hull, Entity ship)
        {
            Part = hull;
            Ship = ship;
        }

        public Hull Part { get; set; }
        public Entity Ship { get; set; }
    }
}