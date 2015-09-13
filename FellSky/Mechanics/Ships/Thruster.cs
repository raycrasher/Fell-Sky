using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Mechanics.Ships
{
    public class Thruster
    {
        public float MaxThrust { get; set; }
        public TimeSpan RampUpTime { get; set; }
        public TimeSpan RampDownTime { get; set; }
    }
}
