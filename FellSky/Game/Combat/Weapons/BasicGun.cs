using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using FellSky.Components;
using FellSky.Game.Ships;
using FellSky.EntityFactories;
using Microsoft.Xna.Framework;
using FellSky.Game.Inventory;
using FellSky.Game.Ships.Parts;

namespace FellSky.Game.Combat.Weapons
{
    public class BasicGun : IWeapon, IInventoryItem, IPersistable
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string TurretId { get; set; }
        public string ProjectileId { get; set; }
        public float DamagePerSecond { get; set; }
        public float TurnRate { get; set; } = MathHelper.Pi;
        public IProjectile Projectile { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public Dictionary<int,ShipPart> Muzzles { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public Dictionary<int, ShipPart> Barrels { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public ShipPart[] FixedParts { get; set; }

        public virtual Entity Install(EntityWorld world, Entity owner, Entity hardpoint)
        {
            var entity = world.CreateEntity();
            var gunComponent = new BasicGunComponent();
            gunComponent.Gun = this;
            gunComponent.Owner = owner;
            entity.AddComponent(hardpoint.GetComponent<Transform>());
            var hardpointComponent = hardpoint.GetComponent<HardpointComponent>();
            entity.AddComponent(hardpointComponent);
            hardpointComponent.InstalledEntity = entity;
            owner.GetComponent<ShipComponent>().Turrets.Add(entity);
            
            entity.AddComponent<IWeaponComponent>(gunComponent);


            var group = CreatePartGroup(TurretId);
            entity.AddComponent(group);
            world.SpawnShipPartGroup(entity, group.PartGroup);

            if (Muzzles == null)
                Muzzles = group.PartGroup.GetNumberedFlaggedParts("Muzzle");
            if (Barrels == null)
                Muzzles = group.PartGroup.GetNumberedFlaggedParts("Barrel");
            if (FixedParts == null)
                FixedParts = group.PartGroup.GetFlaggedParts("Fixed").ToArray();

            var turret = new TurretComponent
            {
                TurnRate = TurnRate,
                FiringArc = hardpointComponent.Hardpoint.FiringArc
            };
            entity.AddComponent(turret);

            // todo: set projectile

            return entity;
        }

        

        public virtual void Fire(EntityWorld world, Entity owner, Entity weapon)
        {
            var weaponComponent = weapon.GetComponent<IWeaponComponent>();
            
            
        }

        public virtual void Uninstall(Entity owner, Entity weaponEntity)
        {
            var shipPartGroup = weaponEntity.GetComponent<ShipPartGroupComponent>();          
            owner.GetComponent<ShipComponent>().Turrets.Remove(weaponEntity);
            weaponEntity.GetComponent<HardpointComponent>().InstalledEntity = null;
            var turret = weaponEntity.GetComponent<TurretComponent>();
            weaponEntity.Delete();
            foreach (var item in shipPartGroup.PartEntities)
            {
                item.Entity.Delete();
            }
            foreach (var item in turret.Barrels)
                item.Delete();
        }

        private static ShipPartGroupComponent CreatePartGroup(string id)
        {
            ShipPartGroup group;
            if(!PartGroups.TryGetValue(id, out group))
            {
                var path = $"Weapons/{id}.json";
                if (!System.IO.File.Exists(path))
                    throw new ArgumentException($"Cannot find PartGroup file: {path}");
                group = Persistence.LoadFromFile<ShipPartGroup>(path);
                
                PartGroups[id] = group;
            }
            return new ShipPartGroupComponent(group);
        }

        private static readonly Dictionary<string, ShipPartGroup> PartGroups = new Dictionary<string, ShipPartGroup>();
    }
}
