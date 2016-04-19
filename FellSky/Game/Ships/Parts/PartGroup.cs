using Artemis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Ships.Parts
{
    public class PartGroup: IComponent
    {
        public PartGroup Parent { get; set; }
        public string Name { get; set; }
        public List<Hull> Hulls { get; set; } = new List<Hull>();
        public List<Thruster> Thrusters { get; set; } = new List<Thruster>();
        public List<WeaponMount> WeaponMounts { get; set; } = new List<WeaponMount>();
        public List<ShipLight> Lights { get; set; } = new List<ShipLight>();
    }
}
