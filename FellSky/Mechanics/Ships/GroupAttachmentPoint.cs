using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Mechanics.Ships
{
    public class GroupAttachmentPoint: ShipPart
    {
        public Transform OriginalTransform { get; set; } = new Transform();
        public Transform Transform { get; set; } = new Transform();
        
    }
}
