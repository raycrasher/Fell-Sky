using Artemis.Interface;
using FellSky.Mechanics.Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.EntityComponents
{
    [Serializable]
    public class ShipComponent: IComponent
    {
        public Ship Ship { get; set; }
        public List<Thruster> Thrusters { get; } = new List<Thruster>();
        public List<Turret> Turrets { get; } = new List<Turret>();
    }
}
