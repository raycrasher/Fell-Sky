using Artemis.Interface;
using FellSky.Models.Ships.Parts;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Ships
{
    public class Ship
    {
        public string GivenName { get; set; }
        public string HullClass { get; set; }
        public string ShipType { get; set; }

        public List<Hull> Hulls { get; set; } = new List<Hull>();
        public List<Thruster> Thrusters { get; set; } = new List<Thruster>();
        public List<WeaponMount> WeaponMounts { get; set; } = new List<WeaponMount>();
        public List<ShipLight> Lights { get; set; } = new List<ShipLight>();
        public List<ModuleSlot> ModuleSlots { get; set; } = new List<ModuleSlot>();

        [JsonIgnore]
        public IEnumerable<ShipPart> Parts { get {
                foreach (var p in Hulls) yield return p;
                if(Thrusters != null) foreach (var p in Thrusters) yield return p;
                if (WeaponMounts != null) foreach (var p in WeaponMounts) yield return p;
                if (Lights != null) foreach (var p in Lights) yield return p;
                if (ModuleSlots != null) foreach (var p in ModuleSlots) yield return p;
                yield return WarpDrive;
                if (Reactors != null) foreach (var p in Reactors) yield return p;
            } }

        public PartGroup CorePartGroup { get; } = new PartGroup { Name = "Ship Core" };
        public WarpDrive WarpDrive { get; set; }

        public List<Reactor> Reactors { get; set; } = new List<Reactor>();

        public float CombatReadiness { get; set; }

        public ShipHandlingParameters Handling { get; set; } = new ShipHandlingParameters();

        [JsonIgnore]
        public IEnumerable<PartGroup> Groups { get {
            return Hulls.Cast<ShipPart>()
                    .Concat(Thrusters)
                    .Concat(WeaponMounts)
                    .Concat(Lights)
                    .Select(p => p.Group)
                    .Distinct()
                    .Where(g => g != null);
        } }

        public Color BaseDecalColor { get; set; } = Color.White;
        public Color TrimDecalColor { get; set; } = Color.White;

        public bool InstallPartGroup(PartGroup group)
        {
            if (group == null) throw new ArgumentNullException(nameof(group));
            if (Hulls.Cast<ShipPart>().Concat(Thrusters).Concat(WeaponMounts).Any(p => p.Group == group)) return false;

            foreach (var part in group.Hulls.Cast<ShipPart>().Concat(group.Thrusters).Concat(WeaponMounts))
                part.Group = group;

            Hulls.AddRange(group.Hulls);
            Thrusters.AddRange(group.Thrusters);
            WeaponMounts.AddRange(group.WeaponMounts);
            Lights.AddRange(group.Lights);
            return true;
        }

        public void RemovePartGroup(PartGroup group)
        {
            Hulls.RemoveAll(h => h.Group == group);
            Thrusters.RemoveAll(t => t.Group == group);
            WeaponMounts.RemoveAll(w => w.Group == group);
            Lights.RemoveAll(l => l.Group == group);
        }

        public void RemovePart(ShipPart part)
        {
            if (part is Hull) Hulls.Remove((Hull)part);
            else if (part is Thruster) Thrusters.Remove((Thruster)part);
            else if (part is WeaponMount) WeaponMounts.Remove((WeaponMount)part);
            else if (part is ShipLight) Lights.Remove((ShipLight)part);
        }

        public void SaveToJsonFile(string filename)
        {
            System.IO.File.WriteAllText( filename, JsonConvert.SerializeObject(this) );
        }

        public static Ship LoadFromJsonFile(string filename)
        {
            return JsonConvert.DeserializeObject<Ship>(System.IO.File.ReadAllText(filename));
        }
    }
}
