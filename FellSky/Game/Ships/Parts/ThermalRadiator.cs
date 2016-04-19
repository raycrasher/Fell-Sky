using Artemis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Ships.Parts
{
    public class ThermalRadiator: IComponent
    {
        public float RadiativeArea { get; set; } // in m^2
        public float DissipationRateBonus { get; set; }
    }
}
