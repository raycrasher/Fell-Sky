using System;
using Artemis;
using Artemis.Interface;
using FellSky.Models.Ships.Parts;
using FellSky.Services;
using Microsoft.Xna.Framework.Content;

namespace FellSky.Components
{
    public class HullComponent: IComponent, IPartComponent<Hull>
    {
        public HullComponent(Hull hull, Entity ship, ISpriteManagerService spriteManager)
        {
            Part = hull;
            Sprite = spriteManager.CreateSpriteComponent(hull.SpriteId);
            Ship = ship;
        }

        public SpriteComponent Sprite { get; set; }
        public Hull Part { get; set; }
        public Entity Ship { get; set; }
    }
}