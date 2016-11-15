using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;

namespace FellSky.Game.Space.MapEffects
{
    public class GravityWell: IMapEffect
    {
        public string Name => "Gravity Well";
        public string IconId => "icon_mapeffect_gravitywell";
        public string Description => "-30% top speed.";

        public void ApplyToMap(SpaceMap map, EntityWorld world)
        {
            throw new NotImplementedException();
        }
    }
}
