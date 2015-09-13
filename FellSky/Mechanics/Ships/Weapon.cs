using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Mechanics.Ships
{
    public class Weapon
    {
        public float Impact { get; set; } // damage to structure
        public float Pierce { get; set; } // piercing damage
        public float Heat { get; set; }   // heat generated on target
        public float Emp { get; set; }    // Emp generated on target
    }
}
