using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using FellSky.Components;
using FellSky.Game.Ships.Parts;

namespace FellSky.Game.Ships.Modules
{
    [Archetype]
    public class Weapon : Module
    {
        public float TurnRate { get; set; }
        public float ShotSpread { get; set; }
        public float FireRate { get; set; }
        public float Recoil { get; set; }

        public WeaponMountType CompatibleHardpoint { get; set; }

        public int NumBarrels { get; set; }
        public float MultiBarrelAlternateFireDelay { get; set; }
        public float VisualRecoilTraverseDistance { get; set; }
        public float VisualRecoilTraverseTime { get; set; }

        public float PowerRequiredPerSecond { get; set; }
        public float HeatGeneratedPerSecond { get; set; }        

        public float Damage { get; set; } // damage per shot

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

        public override Entity Install(EntityWorld world, Entity shipEntity, Hardpoint slot)
        {
            var weaponEntity = base.Install(world, shipEntity, slot);

            var weaponComponent = new WeaponComponent();
            weaponComponent.Weapon = this;
            weaponComponent.Base = world.CreateEntity();
            weaponComponent.Turret = world.CreateEntity();
            weaponEntity.AddChild(weaponComponent.Turret);
            weaponEntity.AddChild(weaponComponent.Base);
            weaponComponent.Barrels = new Entity[NumBarrels];
            for (int i = 0; i < NumBarrels; i++)
            {
                var barrel = world.CreateEntity();
                barrel.AddComponent(new Transform());
                weaponComponent.Barrels[i] = barrel;
                weaponEntity.AddChild(weaponComponent.Turret);
            }

            weaponComponent.Base.AddComponent(new Transform());
            weaponComponent.Turret.AddComponent(new Transform());            

            var moduleComponent = weaponEntity.GetComponent<ModuleComponent>();

            foreach(var upgrade in moduleComponent.InstalledMods.OfType<WeaponUpgrade>().Where(b=>!string.IsNullOrWhiteSpace(b.PartGroupId)))
            {
                switch (upgrade.Type)
                {
                    case WeaponUpgradeType.Barrel:
                        foreach(var barrel in weaponComponent.Barrels)
                        {
                            EntityFactories.ShipEntityFactory.CreatePartGroup(upgrade.PartGroupId).CreateEntities(world, barrel);
                        }
                        break;
                    case WeaponUpgradeType.Mount:
                        break;
                }
            }

            return weaponEntity;
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
    }
}
