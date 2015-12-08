using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Ships.Parts
{
    public class Weapon
    {
        public float Range { get; set; }
        public float Damage { get; set; }
        public float Heat { get; set; }   // heat generated on target
        public float Emp { get; set; }    // Emp generated on target
    }
}
