using Artemis;
using FellSky.Components;

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Ships
{
    public class ShipVariant: IPersistable
    {
        public string HullId { get; set; }
        public string VariantName { get; set; }
        public Color BaseDecalColor { get; set; } = Color.White;
        public Color TrimDecalColor { get; set; } = Color.White;

        public Dictionary<string, string> Weapons { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Modules { get; set; } = new Dictionary<string, string>();

        // TODO: Add per-ship control scheme here.

        public bool ApplyVariant(EntityWorld world, Entity shipEntity)
        {
            var shipComponent = shipEntity.GetComponent<ShipComponent>();
            if (!shipComponent.Ship.Id.Equals(HullId, StringComparison.InvariantCultureIgnoreCase)) return false;

            var ship = shipComponent.Ship;
            shipComponent.Variant = this;

            var model = shipEntity.GetComponent<ShipModelComponent>();
            model.BaseDecalColor = BaseDecalColor;
            model.TrimDecalColor = TrimDecalColor;
            
            // uninstall weapons
            foreach(var entity in shipComponent.Weapons.ToArray())
            {
                entity.GetComponent<WeaponComponent>().Weapon.Uninstall(shipEntity, entity);
            }

            shipComponent.Weapons.Clear();


            var hardpointLookup = shipComponent.Hardpoints.ToDictionary(k => k.GetComponent<HardpointComponent>().Hardpoint.Id);
            foreach(var weapon in Weapons.ToArray())
            {
                CombatEntityFactory.Weapons[weapon.Value].Install(world, shipEntity, hardpointLookup[weapon.Key]);
            }
            foreach (var modules in Modules)
            {
                // TODO: install modules
            }
            return true;
        }
    }
}
