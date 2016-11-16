using Artemis;
using Artemis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public enum ShipAIMode
    {
        StandBy,    // Stay in place, and react if situation changes.
        Travel,     // Travel to another point.
        Escort,      // Try to match speed and heading.

        Flank,      // Try to get to target's blind side while staying out of range and firing on opportunity, then fire at will.
        Assault,    // Try to get the maximum amount of guns to bear on target, then BOOM
        Guard,      // Protect the target. If target is not attacking target, then get in between target and attacker. If not, then move out of the way and protect target against other threats.
        Strafe,     // Go into assault mode and do an alpha strike, then move to flank and turn back for another pass.
        Harass,     // try to stay in maximum possible range while taking pot-shots and avoiding fire
        Retreat,    // try to retreat from the front while fighting
        Rout,        // Full on rout - turn back on enemy and burn as fast as possible.

        

    }

    public class ShipAIComponent: IComponent
    {
        public Entity MainTarget, SubTarget;
        public ShipAIMode Mode;
    }
}
