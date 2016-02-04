using Artemis.Interface;

using Microsoft.Xna.Framework;
using System;

namespace FellSky.Models.Ships.Parts
{
    public sealed class ShipLight: ShipPart
    {
        public Transform Transform { get; set; } = new Transform();
        public string SpriteId { get; set; }
        
        public KeyFrame[] Frames { get; set; } = new[] { new KeyFrame(TimeSpan.FromSeconds(1), Color.LightCyan, 1.0f) };

        public int CurrentFrame { get; set; } = 0;
        public TimeSpan Age { get; set; } = TimeSpan.Zero;
        public Color CurrentColor { get; set; } = Color.LightCyan;
        public float CurrentSize { get; set; } = 1;

        public struct KeyFrame
        {
            public TimeSpan Duration { get; set; }
            public Color Color { get; set; }
            public float Size { get; set; }

            public KeyFrame(TimeSpan duration, Color color, float size)
            {
                Duration = duration;
                Color = color;
                Size = size;
            }
        }
    }
}
