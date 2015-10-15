using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Mechanics.Ships
{
    public class FissionReactor: ShipPart
    {

        public float MaxThermalPower { get; set; }
        public float ThermalPowerRampUp { get; set; }
        public float MaxChargedParticlesProduction { get; set; }
        public float ReactorPoisonAmount { get; set; }
    }
}
