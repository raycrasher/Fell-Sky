using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Mechanics.Ships
{
    public class ShipPart
    {
        public string Name { get; set; }
        public PartGroup Group { get; set; }
        public HashSet<string> Tags { get; set; } = new HashSet<string>();

        public ShipPart()
        {
             Name = Name ?? this.GetType().Name;
        }
    }
}
