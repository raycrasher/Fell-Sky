using FellSky.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace FellSky.Mechanics.Ships
{
    public class Thruster
    {
        public ThrusterData Data { get; set; }
        public ShipSubSprite Sprite { get; set; }
        
        public float MainThrustAmount { get; set; }
        public float BoostThrustAmount { get; set; }
        public float CruiseThrustAmount { get; set; }
    }

    [Serializable]
    public class ThrusterData
    {
        public float Length { get; set; }
        public float Width { get; set; }
        public string ParticleTrailId { get; set; } // not required, can be null for no particle trail.
        public Color Color { get; set; } = Color.White;
        public string MainSpriteId { get; set; }
        public string BoostSpriteId { get; set; }
        public string CruiseThrustId { get; set; }
        public float MaxThrust { get; set; }
        public TimeSpan RampUpTime { get; set; }
        public TimeSpan RampDownTime { get; set; }
    }
}
