using Artemis.Interface;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Space
{
    public class Star: IComponent
    {
        public string StellarClassName { get; set; }
        public Color Color { get; set; }
        public float Temperature { get; set; }
        public float CoronaRadius { get; set; }
        public float PhotosphereRadius { get; set; }
    }
}
