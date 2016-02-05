using Artemis.Interface;
using Microsoft.Xna.Framework;

namespace FellSky.Components
{
    public class DrawBoundingBoxComponent: IComponent
    {
        public Color Color { get; set; } = Color.White;
        public bool IsEnabled { get; set; } = true;
    }
}
