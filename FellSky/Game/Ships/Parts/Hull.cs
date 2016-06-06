using Artemis.Interface;

using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using FellSky.Framework;

namespace FellSky.Game.Ships.Parts
{
    public enum HullColorType
    {
        Hull, BaseDecal, TrimDecal
    }

    /// <summary>
    /// The physical parts of a ship. Determines the graphics and collision data of the ship.
    /// </summary>
    public sealed class Hull: ShipPart, IComponent
    {
        public Hull() { }

        public Hull(string id, Vector2 position, float rotation, Vector2 scale, Vector2 origin, Color color)
        {
            SpriteId = id;
            Transform.Position = position;
            Transform.Scale = scale;
            Transform.Rotation = rotation;
            Transform.Origin = origin;
            Color = color;
        }

        public HullColorType ColorType { get; set; } = HullColorType.Hull;
        public string ShapeId { get; set; }
        //public SpriteEffects SpriteEffect { get; set; }
        public float Health { get; set; } = 100;
    }
}
