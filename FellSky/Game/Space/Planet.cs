using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Space
{
    public enum PlanetType
    {
        Rock,
        GasGiant
    }

    public class Planet
    {
        public PlanetType PlanetType { get; set; }
    }
}
