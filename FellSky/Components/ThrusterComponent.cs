using Artemis;
using Artemis.Interface;
using FellSky.Framework;
using FellSky.Models.Ships.Parts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public class ThrusterComponent: ShipPartComponent<Thruster>
    {
        public ThrusterComponent(Thruster part, Entity ship)
            : base(part,ship)
        {            
        }

        public bool IsThrusting { get; internal set; }
        public SpriteComponent Sprite { get; set; }
        public float AngleCutoff { get; set; } = 0.25f;
        public float ThrustPercentage { get; set; }
    }
}
