using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Combat.Weapons
{
    public enum WeaponClass
    {
        Assault,        // for general use
        CloseSupport,   // use when in close range (half od weapon range)
        PointDefense,   // use against missiles and fighters
        Siege           // use at maximum range against slow ships
    }

    public class WeaponData
    {
        public int NumBarrels { get; set; }
        public float RateOfFire { get; set; }
        public float DamagePerShot { get; set; }
        public float Range { get; set; }

        public float HeatGenerated { get; set; }
        public float PowerDraw { get; set; }
    }
}
