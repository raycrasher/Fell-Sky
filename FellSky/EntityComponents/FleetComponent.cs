using Artemis.Interface;
using FellSky.Mechanics.Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.EntityComponents
{
    public class FleetComponent: IComponent
    {
        public string Name { get; set; } = "Unknown Fleet";
        public List<Ship> Ships { get; set; } = new List<Ship>();
    }
}
