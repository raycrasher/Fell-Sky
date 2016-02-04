using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FellSky.Framework
{
    public class Sprite
    {
        public string Texture { get; set; }
        public string Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int W { get; set; }
        public int H { get; set; }
        public float? OriginX { get; set; }
        public float? OriginY { get; set; }
        public float? Padding { get; set; }
        public string Type { get; set; }
        public string Subtype { get; set; }
        public string Tags { get; set; }
        public List<Sprite> Subsprites { get; set; }
    }
}
