using Artemis.Interface;
using FellSky.Ships.Parts;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Ships
{
    public sealed class Ship: IComponent
    {
        public string GivenName { get; set; }
        public string HullClass { get; set; }
        public string ShipType { get; set; }

        public List<Hull> Hulls { get; } = new List<Hull>();
        public List<Thruster> Thrusters { get; } = new List<Thruster>();
        public List<WeaponMount> WeaponMounts { get; } = new List<WeaponMount>();
        public List<ShipLight> Lights { get; set; } = new List<ShipLight>();
        public List<ModuleSlot> ModuleSlots { get; set; } = new List<ModuleSlot>();

        public PartGroup CorePartGroup { get; } = new PartGroup { Name = "Ship Core" };
        public WarpDrive WarpDrive { get; set; }

        public List<Reactor> Reactors { get; set; } = new List<Reactor>();

        public float CombatReadiness { get; set; }

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

        public bool InstallPartGroup(PartGroup group)
        {
            if (group == null) throw new ArgumentNullException(nameof(group));
            if (Hulls.Cast<ShipPart>().Concat(Thrusters).Concat(WeaponMounts).Any(p => p.Group == group)) return false;

            foreach (var part in group.Hulls.Cast<ShipPart>().Concat(group.Thrusters).Concat(WeaponMounts))
                part.Group = group;

            Hulls.AddRange(group.Hulls);
            Thrusters.AddRange(group.Thrusters);
            WeaponMounts.AddRange(group.WeaponMounts);
            return true;
        }

        public void RemovePartGroup(PartGroup group)
        {
            Hulls.RemoveAll(h => h.Group == group);
            Thrusters.RemoveAll(t => t.Group == group);
            WeaponMounts.RemoveAll(w => w.Group == group);
        }

        public void RemovePart(ShipPart part)
        {
            if (part is Hull) Hulls.Remove((Hull)part);
            else if (part is Thruster) Thrusters.Remove((Thruster)part);
            else if (part is WeaponMount) WeaponMounts.Remove((WeaponMount)part);
        }

        public void SaveToFile(string filename)
        {
            System.IO.File.WriteAllText( filename, JsonConvert.SerializeObject(this) );
        }

        public static Ship LoadFromFile(string filename)
        {
            return JsonConvert.DeserializeObject<Ship>(System.IO.File.ReadAllText(filename));
        }
    }
}
