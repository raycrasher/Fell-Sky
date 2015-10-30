using Artemis.Interface;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace FellSky.EntityComponents
{
    public class MouseControlledTransformComponent: IComponent
    {
        public Matrix TransformationMatrix { get; set; } = Matrix.Invert(Matrix.Identity);
    }
}
