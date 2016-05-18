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
        Weapon_Missile,
        Weapon_Energy,
        Weapon_Ballistic,

        // combo mounts
        Weapon_Universal,   // all weapons
        Weapon_Powered,     // ballistic & energy
        Weapon_Hybrid,      // ballistic & missile
        Weapon_Synergy,     // energy & missile

        // specialty mounts
        Weapon_Spinal,
        Weapon_Beam,
        Weapon_VLS, // vertical launch system,
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
    }
}
