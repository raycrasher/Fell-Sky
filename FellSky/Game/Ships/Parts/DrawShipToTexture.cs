using FellSky.Services;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Ships.Parts
{
    public static class DrawShipToTexture
    {
        public static Texture2D DrawToTexture(this Ship ship)
        {
            var size = ship.CalculateBoundingBox();
            size.Inflate(10, 10);

            var graphics = ServiceLocator.Instance.GetService<GraphicsDevice>();
            var texture = new RenderTarget2D(graphics, (int)size.Width, (int)size.Height);

            graphics.SetRenderTarget(texture);

            

            graphics.SetRenderTarget(null);

            return texture;
        }
    }
}
