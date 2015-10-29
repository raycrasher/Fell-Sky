using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Mechanics.Ships
{
    public class ModuleSlot
    {
        public IModule Module { get; set; }
        public ModuleSize Size { get; set; }
        public Vector2 Position { get; set; }
    }
}
