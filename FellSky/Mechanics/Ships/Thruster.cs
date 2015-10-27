using FellSky.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace FellSky.Mechanics.Ships
{
    public enum ThrusterType
    {
        Main, Cruise, Boost
    }

    public class Thruster: ShipPart
    {
        public Transform Transform { get; set; } = new Transform();
        public float Length { get; set; }
        public float Width { get; set; }
        public string ParticleTrailId { get; set; } // not required, can be null for no particle trail.
        public Color Color { get; set; } = Color.White;
        public ThrusterType ThrusterType { get; set; }

        public string SpriteId { get; set; }
        public float MaxThrust { get; set; }
        public TimeSpan RampUpTime { get; set; }
        public TimeSpan RampDownTime { get; set; }

        [JsonIgnore] public Sprite Sprite { get; set; }

        public float ThrustPercent { get; set; }
    }
}
