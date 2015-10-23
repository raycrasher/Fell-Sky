using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Mechanics.Ships
{
    [Serializable]
    public class Turret
    {
        public WeaponHardpoint Hardpoint { get; set; }
        public float CurrentFacing { get; set; }
    }
}
