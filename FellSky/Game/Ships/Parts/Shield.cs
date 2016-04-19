using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Ships.Parts
{
    public enum ShieldType
    {
        Field, EnergyCoat, Barrier
    }

    public class Shield
    {
        public float MaxShieldStrength { get; set; }
        public float CurrentShieldStrength { get; set; }
        public ShieldType Type { get; set; }
    }
}
