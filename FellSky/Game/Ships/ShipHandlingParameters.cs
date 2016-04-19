using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Ships
{
    public class ShipHandlingParameters
    {
        public float AngularTorque { get; set; }
        public float MaxAngularVelocity { get; set; }
        public float ForwardForce { get; set; }
        public float ManeuverForce { get; set; } // side, back

        public float LinearDamping { get; set; }
        public float AngularDamping { get; set; }


    }
}
