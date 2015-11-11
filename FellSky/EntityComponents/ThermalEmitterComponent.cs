using Artemis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.EntityComponents
{
    public class ThermalEmitterComponent: IComponent
    {
        public float Temperature { get; set; }
    }
}
