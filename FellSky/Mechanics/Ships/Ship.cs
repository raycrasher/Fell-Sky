using Artemis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Mechanics.Ships
{
    public class Ship
    {
        public List<Hull> Hulls { get; } = new List<Hull>();
        public List<WeaponHardpoint> WeaponHardpoints { get; } = new List<WeaponHardpoint>();
    }
}
