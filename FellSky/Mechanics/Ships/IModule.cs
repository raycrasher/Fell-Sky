using FellSky.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Mechanics.Ships
{
    [Flags]
    public enum ModuleSize
    {
        Small = 2 << 0,
        Medium = 2 << 1,
        Large = 2 << 2,
        Huge = 2 << 3
    }

    [Flags]
    public enum ModuleClass
    {
        Critical = 2 << 0,
        LifeSupport = 2 << 1,
        Power = 2 << 2,
        Command =  2 << 3,
        Thermal = 2 << 4,
        Propulsion = 2 << 5,
        Maneuver = 2 << 6,
        Computing = 2 << 7,
        Communications = 2 << 8,
        Structural = 2 << 9,
        Sensor = 2 << 10,
        Weapons = 2 << 11,
        Defense = 2 << 12,
        Support = 2 << 13,
        Auxiliary = 2 << 14,
        Science = 2 << 15,
        Industry = 2 << 16,
        Storage = 2 << 17,
        Leisure = 2 << 18
    }

    public interface IModule
    {
        string IconSpriteId { get; set; }
        Sprite IconSprite { get; set; }
        ModuleSize Size { get; set; }
        string Name { get; }
        string Description { get; set; }
        ModuleClass Class { get; }
    }
}
