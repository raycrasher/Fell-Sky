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
        public string Name { get; set; }
        
        public List<ShipPart> Parts { get; } = new List<ShipPart>();
        public List<WeaponHardpoint> WeaponHardpoints { get; } = new List<WeaponHardpoint>();
        public ShipHandlingParameters Handling { get; set; } = new ShipHandlingParameters();
    }
}
