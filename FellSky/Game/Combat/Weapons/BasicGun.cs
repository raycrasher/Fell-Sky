using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using FellSky.Components;
using FellSky.Game.Ships;
using FellSky.EntityFactories;

namespace FellSky.Game.Combat.Weapons
{
    public class BasicGun : IWeapon
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string TurretId { get; set; }
        public string ProjectileId { get; set; }
        public float DamagePerSecond { get; set; }

        public virtual void Spawn(EntityWorld world, Entity owner, Entity entity)
        {
            var gunComponent = new BasicGunComponent();
            gunComponent.Gun = this;
            gunComponent.Owner = owner;
            
            entity.AddComponent<IWeaponComponent>(gunComponent);
            var group = GetPartGroup(TurretId);
            entity.AddComponent(group);
            world.SpawnShipPartGroup(entity, group.PartGroup);
        }

        public virtual void Fire(EntityWorld world, Entity owner, Entity weapon)
        {
            
        }

        private static ShipPartGroupComponent GetPartGroup(string id)
        {
            ShipPartGroupComponent component;
            if(!PartGroups.TryGetValue(id, out component))
            {
                var path = $"Weapons/{id}.json";
                if (!System.IO.File.Exists(path))
                    throw new ArgumentException($"Cannot find PartGroup file: {path}");
                var group = Newtonsoft.Json.JsonConvert.DeserializeObject<ShipPartGroup>(System.IO.File.ReadAllText(path), JsonSettings);
                component = new ShipPartGroupComponent(group);
                PartGroups[id] = component;
            }
            return component;
        }

        private static readonly Dictionary<string, ShipPartGroupComponent> PartGroups = new Dictionary<string, ShipPartGroupComponent>();

        private static readonly Newtonsoft.Json.JsonSerializerSettings JsonSettings = new Newtonsoft.Json.JsonSerializerSettings
        {
            TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto,
            PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.All
        };
    }
}
