using Artemis.Interface;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace FellSky.EntityComponents
{
    public class MouseControlledTransformComponent: IComponent
    {
        public Transform InitialTransform { get; set; }
        public Matrix TransformationMatrix { get; set; } = Matrix.Identity;
    }
}
