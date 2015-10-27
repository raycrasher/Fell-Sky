using Artemis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Mechanics.Ships
{
    public class Ship: IComponent
    {
        public string GivenName { get; set; }
        public string HullClass { get; set; }
        public string ShipType { get; set; }

        public List<Hull> Hulls { get; } = new List<Hull>();
        public List<Thruster> Thrusters { get; } = new List<Thruster>();
        public List<WeaponMount> WeaponMounts { get; } = new List<WeaponMount>();
        public WarpDrive WarpDrive { get; set; }

        public ShipHandlingParameters Handling { get; set; } = new ShipHandlingParameters();
    }
}
