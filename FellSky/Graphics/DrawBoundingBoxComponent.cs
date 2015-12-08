using Artemis.Interface;
using Microsoft.Xna.Framework;

namespace FellSky.Graphics
{
    public class DrawBoundingBoxComponent: IComponent
    {

        public DrawBoundingBoxComponent(FloatRect boundingBox)
        {
            BoundingBox = boundingBox;
        }

        public Color Color { get; set; } = Color.White;
        public FloatRect BoundingBox { get; set; }
        public bool IsEnabled { get; set; } = true;
    }
}
