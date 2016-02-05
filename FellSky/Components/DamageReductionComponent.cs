using Artemis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public class DamageReductionComponent: IComponent
    {
        public float Kinetic { get; set; }  // bullets
        public float Energy { get; set; }   // lasers, fire, etc.
        public float Chemical { get; set; } // attack on molecular-level structures
        public float Warp { get; set; }     // magnetic and gravity fields
    }
}
