using Artemis.Interface;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Ships.Parts
{
    public class ModuleSlot: ShipPart, IComponent
    {
        public IModule Module { get; set; }
        public ModuleSize Size { get; set; }
        public Vector2 Position { get; set; }
    }
}
