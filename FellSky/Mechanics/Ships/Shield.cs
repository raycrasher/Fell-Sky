using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Mechanics.Ships
{
    public enum ShieldType
    {
        Field, EnergyCoat, Barrier
    }

    public class Shield
    {
        public ShieldType Type { get; set; }
    }
}
