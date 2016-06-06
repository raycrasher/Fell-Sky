using Artemis.Interface;
using LibRocketNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    // this component is meant to be attached to the entity at the firing point.
    public class BeamRendererComponent: IComponent
    {
        public Color MainColor;
        public Color CoreColor;
        public float Length;
    }
}
