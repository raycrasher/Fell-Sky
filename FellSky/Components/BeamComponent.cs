using Artemis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using Microsoft.Xna.Framework;
using FellSky.Framework;

namespace FellSky.Components
{
    public class BeamComponent : IComponent
    {
        public Color Color;
        public float DamagePerSecond;
        public Entity Muzzle;
    }
}
