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
    public class ThrusterComponent: IComponent, IPartComponent<Thruster>
    {
        public SpriteComponent Sprite { get; set; }
        public float ThrustPercentage { get; set; }
        public bool IsDamaged { get; set; }
        public Thruster Part { get; set; }
    }
}
