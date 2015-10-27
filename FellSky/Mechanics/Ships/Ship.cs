using Artemis.Interface;
using FellSky.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Mechanics.Ships
{
    public class Ship: IComponent
    {
        public string GivenName { get; set; }
        public string HullClass { get; set; }
        public string ShipType { get; set; }

        public List<Hull> Hulls { get; } = new List<Hull>();
        public List<Thruster> Thrusters { get; } = new List<Thruster>();
        public List<WeaponMount> WeaponMounts { get; } = new List<WeaponMount>();
        public List<ShipLight> Lights { get; set; } = new List<ShipLight>();

        public PartGroup CorePartGroup { get; } = new PartGroup { Name = "Ship Core" };
        public WarpDrive WarpDrive { get; set; }

        public ShipHandlingParameters Handling { get; set; } = new ShipHandlingParameters();

        public void SetColor(Color c, HullColorType type)
        {
            var baseColorHsl = c.ToHSL();
            foreach (var hull in Hulls.Where(i => i.ColorType == type))
            {
                var hsl = hull.Color.ToHSL();
                var newColorHsl = new ColorHSL(baseColorHsl.Hue, baseColorHsl.Saturation, hsl.Luminosity + baseColorHsl.Luminosity / 2);
                hull.Color = newColorHsl.ToRgb();
            }
        }

        public IEnumerable<PartGroup> Groups { get {
            return Hulls.Cast<ShipPart>()
                    .Concat(Thrusters)
                    .Concat(WeaponMounts)
                    .Select(p => p.Group)
                    .Distinct()
                    .Where(g => g != null);
        } }

        public bool InstallHullGroup(PartGroup group)
        {
            if (group == null) throw new ArgumentNullException(nameof(group));
            if (Hulls.Cast<ShipPart>().Concat(Thrusters).Concat(WeaponMounts).Any(p => p.Group == group)) return false;

            Hulls.AddRange(group.Hulls);
            Thrusters.AddRange(group.Thrusters);
            WeaponMounts.AddRange(group.WeaponMounts);

            return true;
        }

        public void RemoveHullGroup(PartGroup group)
        {
            Hulls.RemoveAll(h => h.Group == group);
            Thrusters.RemoveAll(t => t.Group == group);
            WeaponMounts.RemoveAll(w => w.Group == group);
        }
    }
}
