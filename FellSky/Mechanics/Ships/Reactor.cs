using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Mechanics.Ships
{
    public class Reactor
    {
        public float FuelLevel { get; set; }
        public float CurrentPower { get; set; }
        public float MaxPower { get; set; }
        public float HeatPerUnitPower { get; set; }
        public float FuelPerUnitPower { get; set; }

        public string FuelResourceId { get; set; }
    }
}
