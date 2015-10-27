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

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
    public class ShipModuleAttribute: Attribute
    {
        public ShipModuleAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }

    public interface IShipModule
    {
        string SpriteId { get; set; }
        Sprite Sprite { get; set; }
        ModuleSize Size { get; set; }
        string Type { get; }
    }
}
