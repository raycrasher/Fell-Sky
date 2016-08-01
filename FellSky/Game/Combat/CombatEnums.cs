using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Combat
{
    public enum WeaponStatus
    {
        Disabled, Ready, Cycling, Reloading
    }

    public enum WeaponAction
    {
        Automatic
    }

    public enum WeaponBarrelStatus
    {
        Idle, Recoiling, Cycling
    }
}
