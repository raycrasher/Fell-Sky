using FellSky.Common;
using FellSky.Graphics;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FellSky.Mechanics.Ships
{
    public enum HullColorType
    {
        None, Base, Trim
    }

    /// <summary>
    /// The physical parts of a ship. Determines the graphics and collision data of the ship.
    /// </summary>
    public class Hull: ShipPart
    {
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
    }
}
