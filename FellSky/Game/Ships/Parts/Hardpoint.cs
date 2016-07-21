using Artemis;
using Artemis.Interface;
using FellSky.Game.Ships.Modules;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Ships.Parts
{
    public enum HardpointType
    {
        Missile,
        Energy,
        Ballistic,

        // combo mounts
        Universal,   // all weapons
        Powered,     // ballistic & energy
        Composite,      // ballistic & missile
        Hybrid,     // energy & missile

        // specialty mounts
        Spinal,
        Beam,
        VLS, // vertical launch system,

        Sensor,
        Thruster,
        PowerCore,

        Module,
    }

    public enum HardpointSize
    {
        Small, Medium, Large, Huge
    }

    public class Hardpoint
    {
        public Module Module { get; set; }
        public HardpointSize Size { get; set; }
        public HardpointType Type { get; set; }
        public Hull Hull { get; set; }

        public float FiringArc { get; set; } // in radians

        public void AddToEntity(Entity ship, Entity entity)
        {

        }
    }
}
