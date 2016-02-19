using Artemis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Models.Ships.Parts
{
    public class RevoluteJointBody: ShipPart
    {
        public float LowerAngleLimit { get; set; }
        public float UpperAngleLimit { get; set; }
        public bool AngleLimitEnabled { get; set; } = false;
    }
}
