using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Ships.Parts
{


    [Flags]
    public enum WeaponMountType
    {
        Beam = 2 << 0,
        Energy = 2 << 1,
        Missile = 2 << 2,
        Ballistic = 2 << 3,
    }

    public class WeaponMount: ShipPart
    {
        public bool IsBuiltIn { get; set; }
        public float TraverseArcStart { get; set; }
        public float TraverseArcEnd { get; set; }
        public ModuleSize Size { get; set; }
        public WeaponMountType Type { get; set; }

        public float CurrentFacing { get; set; }
        public Weapon MountedWeapon { get; set; }
    }
}
