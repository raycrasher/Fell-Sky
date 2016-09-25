using Artemis;
using Artemis.Interface;
using FellSky.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FellSky.Components
{
    public class GenericDrawableComponent: IComponent
    {
        public delegate void DrawDelegate(GraphicsDevice device, SpriteBatch batch, Entity entity);

        public GenericDrawableComponent(DrawDelegate d)
        {
            DrawFunction = d;
        }

        public DrawDelegate DrawFunction;
    }
}
