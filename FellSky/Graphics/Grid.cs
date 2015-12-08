using Artemis.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Graphics
{
    public class GridComponent: IComponent
    {
        public Color GridColor { get; set; } = Color.White;
        public Vector2 GridSize { get; set; } = new Vector2(50);
        public float Parallax { get; set; } = 1;
    }
}
