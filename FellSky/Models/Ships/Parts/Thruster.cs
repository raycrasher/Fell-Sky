
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace FellSky.Models.Ships.Parts
{
    public enum ThrusterType
    {
        Main, Maneuver, Boost
    }

    public class Thruster: ShipPart
    {
        public float Length { get; set; }
        public float Width { get; set; }
        public string ParticleTrailId { get; set; } // not required, can be null for no particle trail.
        public Color Color { get; set; } = Color.White;
        public ThrusterType ThrusterType { get; set; }

        public float MaxThrust { get; set; }
        public TimeSpan RampUpTime { get; set; }
        public TimeSpan RampDownTime { get; set; }
    }
}
