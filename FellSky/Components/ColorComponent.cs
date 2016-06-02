using Artemis.Interface;
using Microsoft.Xna.Framework;

namespace FellSky.Components
{
    public class ColorComponent: IComponent
    {
        public Color Color;
        public ColorComponent(Color color)
        {
            Color = color;
        }
        public ColorComponent()
        {
            Color = Color.White;
        }
    }
}
