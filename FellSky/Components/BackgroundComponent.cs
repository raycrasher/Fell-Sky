using Artemis.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public class BackgroundComponent: IComponent
    {
        public float Depth = 0.5f;
        public float Parallax = 0.5f;

        public Color Color = Color.White;
        public SpriteEffects SpriteEffect;
        public bool FillViewPort = false;
        public bool IsAdditive = false;
    }
}
