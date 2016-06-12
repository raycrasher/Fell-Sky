using Artemis;
using Artemis.Interface;
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
    }

    public enum HardpointSize
    {
        Small, Medium, Large, Huge
    }

    public interface IHardpointMountable
    {
        Hardpoint CurrentHardpoint { get; set; }
        HardpointSize Size { get; }
        bool CanMountToHardpoint(HardpointType type);
    }

    public class Hardpoint
    {
        public IHardpointMountable Module { get; set; }
        public HardpointSize Size { get; set; }
        public HardpointType Type { get; set; }
        public Hull Hull { get; set; }

        public float FiringArc { get; set; } // in radians

        public void AddToEntity(Entity ship, Entity entity)
        {

        }
    }
}
