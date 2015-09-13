using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Mechanics.Ships
{
    public class Reactor
    {
        public float MaxPower { get; set; }
        public string FuelId { get; set; }
        public float FuelUsagePerUnitPower { get; set; }
    }
}
