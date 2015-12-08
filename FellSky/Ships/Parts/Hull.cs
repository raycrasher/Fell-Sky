using Artemis.Interface;
using FellSky.Graphics;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FellSky.Ships.Parts
{
    public enum HullColorType
    {
        None, Base, Trim
    }

    /// <summary>
    /// The physical parts of a ship. Determines the graphics and collision data of the ship.
    /// </summary>
    public class Hull: ShipPart, IComponent
    {
        public Hull() { }

        public Hull(string id, Vector2 position, int rotation, Vector2 scale, Vector2 origin, Color color)
        {
            SpriteId = id;
            Sprite = SpriteManager.Sprites[id];
            Transform.Position = position;
            Transform.Scale = scale;
            Transform.Rotation = rotation;
            Transform.Origin = origin;
            Color = color;
        }

        [JsonIgnore]
        public Sprite Sprite { get; set; }
        public Color Color { get; set; }
        public HullColorType ColorType { get; set; }

        public string SpriteId { get; set; }

        public Transform Transform { get; set; } = new Transform();

        [JsonIgnore]
        public FarseerPhysics.Collision.Shapes.Shape Shape { get; set; }
        public string ShapeId { get; set; }

        [JsonIgnore]
        public FarseerPhysics.Dynamics.Fixture Fixture { get; set; }
        public FloatRect BoundingBox => new FloatRect(Vector2.Zero, new Vector2(Sprite.TextureRect.Width, Sprite.TextureRect.Height));
    }
}
