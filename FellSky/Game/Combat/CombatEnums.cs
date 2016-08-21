using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Combat
{
    public enum WeaponStatus
    {
        Disabled, Ready, ContinuousFiring, Cycling, BurstCycling, Reloading
    }

    public enum WeaponAction
    {
        Automatic,
        ContinuousFire // use for continuous beams
    }

    public enum WeaponBarrelStatus
    {
        Idle, Recoiling, Cycling
    }
}
