using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Ships.Parts
{
    public class GroupMount: ShipPart
    {
        public Transform OriginalTransform { get; set; } = new Transform();
        public Transform Transform { get; set; } = new Transform();
        
    }
}
