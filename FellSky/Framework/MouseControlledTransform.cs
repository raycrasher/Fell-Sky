using Artemis.Interface;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace FellSky.Framework
{
    public class MouseControlledTransform: IComponent
    {
        public Transform InitialTransform { get; set; }
    }
}
