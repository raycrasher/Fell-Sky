using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Mechanics.Ships
{
    [Serializable]
    public abstract class ShipPart
    {
        public Transform Transform { get; set; } = new Transform();
        public string Name { get; set; }
        public string SpriteGroup { get; set; }
        public float Mass { get; set; }
    }
}
