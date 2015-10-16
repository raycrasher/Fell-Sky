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
        public string Name { get; set; }
        
        public ShipSprite Sprite { get; set; } = new ShipSprite();
        public List<Hull> Hulls { get; } = new List<Hull>();
        public List<WeaponHardpoint> WeaponHardpoints { get; } = new List<WeaponHardpoint>();
        public ShipHandlingParameters Handling { get; set; } = new ShipHandlingParameters();
    }
}
