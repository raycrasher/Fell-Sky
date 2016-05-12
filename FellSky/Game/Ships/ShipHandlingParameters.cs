using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Ships
{
    public class ShipHandlingParameters
    {
        public float AngularTorque { get; set; } = 0.2f;
        public float ForwardForce { get; set; } = 2;
        public float ManeuverForce { get; set; } = 1;   // side, back

        public float LinearDamping { get; set; } = 1;
        public float AngularDamping { get; set; } = 1;


    }
}
