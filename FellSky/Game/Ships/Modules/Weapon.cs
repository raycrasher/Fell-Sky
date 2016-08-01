using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using FellSky.Components;
using FellSky.Game.Ships.Parts;
using FellSky.Game.Inventory;
using FellSky.Game.Combat;

namespace FellSky.Game.Ships.Modules
{
    [Archetype]
    public class Weapon : Module, IInventoryItem
    {
        public float TurnRate { get; set; }
        public float ShotSpread { get; set; }
        public float FireRate { get; set; } // shots per second
        public float ReloadRate { get; set; }

        public WeaponMountType CompatibleHardpoint { get; set; }

        public int NumBarrels { get; set; }

        public float VisualRecoilMuzzleTravelDistance { get; set; }
        public float VisualRecoilSpeed { get; set; }
        public float VisualRecoilCycleSpeed { get; set; }

        public float Damage { get; set; } // damage per shot

        public string Name { get; set; }
        public string Description { get; set; }
        public float DamagePerSecond => Damage * FireRate;
        public string ProjectileId { get; set; }
        public string TurretId { get; set; }

        public int MagazineSize { get; set; }

        public WeaponAction Action { get; set; }
        public bool IsMultiBarrelAlternateFire { get; set; }

        public override bool CanInstall(Entity shipEntity, Hardpoint slot)
        {
            if (slot.Size != Size) return false;
            // TODO: Take into account weapon mount type
            return true;
        }

        public override bool CanInstallUpgrade(Entity shipEntity, Entity moduleEntity, IModuleUpgrade upgrade)
        {
            return upgrade is WeaponUpgrade;
        }

        public override Entity Install(EntityWorld world, Entity shipEntity, Entity hardpointEntity)
        {
            var weaponEntity = base.Install(world, shipEntity, hardpointEntity);
            var hardpointComponent = hardpointEntity.GetComponent<HardpointComponent>();
            var weaponComponent = new WeaponComponent();
            weaponComponent.Weapon = this;
            weaponComponent.Mount = world.CreateEntity();
            weaponComponent.Turret = world.CreateEntity();
            weaponEntity.AddChild(weaponComponent.Turret);
            weaponEntity.AddChild(weaponComponent.Mount);
            weaponEntity.AddComponent(weaponComponent);
            weaponComponent.Barrels = new Entity[NumBarrels];
            for (int i = 0; i < NumBarrels; i++)
            {
                var barrel = world.CreateEntity();
                barrel.AddComponent(new Transform());
                weaponComponent.Barrels[i] = barrel;
                weaponEntity.AddChild(weaponComponent.Turret);
            }

            weaponComponent.Mount.AddComponent(new Transform());
            weaponComponent.Turret.AddComponent(new Transform());
            weaponComponent.Turret.AddComponent(new TurretComponent {
                DesiredRotation = 0,
                FiringArc = hardpointComponent.Hardpoint.Traverse,
                TurnRate = TurnRate,
                IsEnabled = true,
                Rotation = 0,
                Weapon = weaponEntity,
                Ship = shipEntity
            });

            var moduleComponent = weaponEntity.GetComponent<ModuleComponent>();

            foreach(var upgrade in moduleComponent.InstalledMods.OfType<WeaponUpgrade>().Where(b=>!string.IsNullOrWhiteSpace(b.PartGroupId)))
            {
                upgrade.Apply(this, world, weaponEntity);
            }

            var shipComponent = shipEntity.GetComponent<ShipComponent>();
            shipComponent.Weapons.Add(weaponEntity);

            return weaponEntity;
        }

        public override void Uninstall(Entity shipEntity, Entity weaponEntity)
        {
            base.Uninstall(shipEntity, weaponEntity);
            var shipComponent = shipEntity.GetComponent<ShipComponent>();
            shipComponent.Weapons.Remove(weaponEntity);
        }
    }

    public enum WeaponMountType
    {
        Ballistic, Energy, Missile, Vls
    }

    public enum WeaponUpgradeType
    {
        Mount, Barrel, Turret, Projectile, Misc
    }

    public class WeaponUpgrade: IModuleUpgrade
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public WeaponUpgradeType Type { get; set; }
        public string PartGroupId { get; set; }

        public virtual void Apply(Weapon weapon, EntityWorld world, Entity weaponEntity)
        {
            var weaponComponent = weaponEntity.GetComponent<WeaponComponent>();
            switch (Type)
            {
                case WeaponUpgradeType.Barrel:
                    foreach (var barrel in weaponComponent.Barrels)
                    {
                        EntityFactories.ShipEntityFactory.CreatePartGroup(PartGroupId).CreateEntities(world, barrel);
                    }
                    break;
                case WeaponUpgradeType.Mount:
                    EntityFactories.ShipEntityFactory.CreatePartGroup(PartGroupId).CreateEntities(world, weaponComponent.Mount);
                    break;
                case WeaponUpgradeType.Turret:
                    EntityFactories.ShipEntityFactory.CreatePartGroup(PartGroupId).CreateEntities(world, weaponComponent.Turret);
                    break;
            }
        }
    }
}
