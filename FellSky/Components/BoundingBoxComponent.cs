using Artemis.Interface;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public class BoundingBoxComponent: IComponent
    {
        public BoundingBoxComponent() { }
        public BoundingBoxComponent(FloatRect box)
        {
            Box = box;
        }
        public FloatRect Box { get; set; }
    }
}
