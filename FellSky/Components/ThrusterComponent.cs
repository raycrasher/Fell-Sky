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
        public SpriteComponent Sprite { get; set; }
        public float ThrustPercentage { get; set; }
        public AngularDirection TurnDirection { get; set; }
    }
}
