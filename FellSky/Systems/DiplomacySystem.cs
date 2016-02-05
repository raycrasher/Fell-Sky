using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using FellSky.Components;

namespace FellSky.Systems
{
    public class DiplomacySystem: Artemis.System.IntervalEntitySystem
    {
        public DiplomacySystem(TimeSpan interval)
            : base(interval, Aspect.All(typeof(FactionComponent)))
        {
        }

        public bool IsFriendlyFirePossibleBetween(FactionComponent factionA, FactionComponent factionB)
        {
            return true;
        }
    }
}
