using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;

namespace FellSky.Game.Space.MapEffects
{
    public class DeepSpace : IMapEffect
    {
        public string Name => "Empty space";
        public string IconId => "icon_mapeffect_emptyspace";
        public string Description => "+100% sensor range.\nShips can use short-range warp in battle.";

        public void ApplyToMap(SpaceMap map, EntityWorld world)
        {
            
        }
    }
}
