using Artemis;
using Artemis.Interface;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.EntityComponents
{
    public class GenericDrawableComponent: IComponent
    {
        public delegate void DrawDelegate(GraphicsDevice device, SpriteBatch batch, Entity entity);
        public DrawDelegate DrawFunction;

        public float Parallax { get; set; } = 1;
    }
}
