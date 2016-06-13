using Artemis;
using FellSky.Components;
using FellSky.EntityFactories;
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

        public void ApplyVariant(EntityWorld world, Entity shipEntity)
        {
            var shipComponent = shipEntity.GetComponent<ShipComponent>();
            var ship = shipComponent.Ship;
            shipComponent.Variant = this;
            shipComponent.BaseDecalColor = BaseDecalColor;
            shipComponent.TrimDecalColor = TrimDecalColor;
            
            // uninstall weapons
            foreach(var entity in shipComponent.Weapons)
            {
                entity.DeleteWithChildren();
            }

            shipComponent.Weapons.Clear();

            foreach(var weapon in Weapons)
            {
                // TODO: install weapons
            }
            foreach (var modules in Modules)
            {
                // TODO: install modules
            }

        }
    }
}
