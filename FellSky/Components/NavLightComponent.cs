using Artemis;
using Artemis.Interface;
using FellSky.Game.Ships.Parts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public class NavLightComponent: ShipPartComponent<NavLight>
    {
        public NavLightComponent(NavLight light, Entity ship)
            : base(light, ship)
        {
        }
    }
}
