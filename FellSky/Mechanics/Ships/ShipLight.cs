using FellSky.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace FellSky.Mechanics.Ships
{
    public class ShipLight
    {
        [Newtonsoft.Json.JsonIgnore]
        public Sprite Sprite { get; set; }

        public string SpriteId { get; set; }
        public Color CurrentColor { get; set; } = Color.LightCyan;
        public KeyFrame[] Frames { get; set; } = new[] { new KeyFrame(TimeSpan.FromSeconds(1), Color.LightCyan) };
        public int CurrentFrame { get; set; }
        public TimeSpan Age { get; set; }

        public struct KeyFrame
        {
            public TimeSpan Duration;
            public Color Color { get; set; }

            public KeyFrame(TimeSpan duration, Color color)
            {
                Duration = duration;
                Color = color;
            }
        }
    }
}
