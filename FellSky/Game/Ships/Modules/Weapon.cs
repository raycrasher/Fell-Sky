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

using FellSky.Services;
using FellSky.Game.Combat.Projectiles;

namespace FellSky.Game.Ships.Modules
{
    [Archetype]
    public class Weapon : Module, IInventoryItem
    {
        public const string AnimateWeaponCycleFlag = "animate-weaponcycle";

        public float TurnRate { get; set; }
        public float ShotSpread { get; set; }
        public float FireRate { get; set; } // shots per second
        public int BurstSize { get; set; } = 1;
        public float ReloadRate { get; set; }

        public WeaponMountType CompatibleHardpoint { get; set; }

        public float VisualRecoilMuzzleTravelDistance { get; set; }
        public float VisualRecoilSpeed { get; set; }
        public float VisualRecoilCycleSpeed { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string ProjectileId { get; set; }

        public int MagazineSize { get; set; }

        public WeaponAction Action { get; set; }
        public bool IsMultiBarrelAlternateFire { get; set; }

        public string MountModel { get; set; }
        public string TurretModel { get; set; }
        public string BarrelModel { get; set; }

        public bool UsesFrameAnimation { get; set; }
        public float AnimateWeaponCycleFps { get; set; } = 1;
        public float BurstRoF { get; set; } = 0;
        public bool NeedsAmmo { get; set; } = false;

        public override bool CanInstallToHardpoint(Entity shipEntity, Hardpoint slot)
        {
            if (slot.Size != Size) return false;
            switch (slot.Type)
            {
                case HardpointType.Universal:
                    return true;
                case HardpointType.Powered:
                    return CompatibleHardpoint == WeaponMountType.Ballistic || CompatibleHardpoint == WeaponMountType.Energy;
                case HardpointType.Composite:
                    return CompatibleHardpoint == WeaponMountType.Ballistic || CompatibleHardpoint == WeaponMountType.Missile;
                case HardpointType.Hybrid:
                    return CompatibleHardpoint == WeaponMountType.Energy || CompatibleHardpoint == WeaponMountType.Missile;
                case HardpointType.Ballistic:
                    return CompatibleHardpoint == WeaponMountType.Ballistic;
                case HardpointType.Beam:
                    return CompatibleHardpoint == WeaponMountType.Energy; //TODO: Check for beam cannon here
                case HardpointType.Energy:
                    return CompatibleHardpoint == WeaponMountType.Energy;
                case HardpointType.Missile:
                    return CompatibleHardpoint == WeaponMountType.Missile;
            }
            return false;
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
            weaponComponent.Barrels = new Entity[0];

            weaponComponent.Mount.AddComponentFromPool<Transform>();
            weaponComponent.Turret.AddComponentFromPool<Transform>();
            weaponComponent.Turret.AddComponent(new TurretComponent {
                DesiredRotation = 0,
                FiringArc = hardpointComponent.Hardpoint.Traverse,
                TurnRate = TurnRate,
                IsEnabled = true,
                Rotation = 0,
                Weapon = weaponEntity,
                Ship = shipEntity
            });

            var parts = new List<Entity>();

            if (!string.IsNullOrEmpty(MountModel))
            {
                ShipEntityFactory.GetShipModel(MountModel).CreateChildEntities(world, weaponComponent.Mount);
                parts.AddRange(weaponComponent.Mount.GetChildren());
            }
            if (!string.IsNullOrEmpty(TurretModel))
            {
                ShipEntityFactory.GetShipModel(TurretModel).CreateChildEntities(world, weaponComponent.Turret);
                parts.AddRange(weaponComponent.Turret.GetChildren());
            }
            if (!string.IsNullOrEmpty(BarrelModel))
            {
                foreach (var barrel in weaponComponent.Barrels)
                {
                    ShipEntityFactory.GetShipModel(BarrelModel).CreateChildEntities(world, barrel);
                    parts.AddRange(barrel.GetChildren());
                }
            }
            else
            {
                var mounts = weaponComponent.Mount.GetChildren().Concat(weaponComponent.Turret.GetChildren()).Where(e => e.GetComponent<IShipPartComponent>().Part.Name.StartsWith("muzzle", StringComparison.InvariantCultureIgnoreCase)).ToArray();
                weaponComponent.Barrels = mounts.Select(m => {
                    var barrel = world.CreateEntity();
                    barrel.AddComponentFromPool<Transform>();
                    var barrelComponent = new WeaponBarrelComponent
                    {
                        Muzzle = m
                    };
                    barrel.AddComponent(barrelComponent);
                    weaponComponent.Turret.AddChild(barrel);
                    return barrel;
                }).ToArray();
                parts.AddRange(weaponComponent.Barrels);
            }

            var moduleComponent = weaponEntity.GetComponent<ModuleComponent>();

            foreach(var upgrade in moduleComponent.InstalledMods.OfType<WeaponUpgrade>().Where(b=>!string.IsNullOrWhiteSpace(b.PartGroupId)))
            {
                upgrade.Apply(this, world, weaponEntity);
            }

            var shipComponent = shipEntity.GetComponent<ShipComponent>();
            shipComponent.Weapons.Add(weaponEntity);

            weaponComponent.Status = WeaponStatus.Ready;

            weaponComponent.Projectile = CombatEntityFactory.Projectiles[ProjectileId];
            weaponComponent.Owner = shipEntity;

            var spriteManager = ServiceLocator.Instance.GetService<ISpriteManagerService>();

            foreach(var entity in weaponEntity.GetDescendants().Where(w=>w.GetComponent<IShipPartComponent>()?.Part.Flags?.Contains(AnimateWeaponCycleFlag) ?? false))
            {
                var frameComponent = spriteManager.CreateFrameAnimationComponent(entity.GetComponent<SpriteComponent>().Name, AnimateWeaponCycleFps);
                entity.AddComponent(frameComponent);
                weaponEntity.RegisterEvent(EventId.WeaponFire, (o, e) => frameComponent.Play());
            }

            if(weaponComponent.Projectile is Beam)
            {
                foreach(var muzzle in weaponComponent.Barrels.Select(b => b.GetComponent<WeaponBarrelComponent>().Muzzle))
                {
                    muzzle.AddComponent(new BeamEmitterComponent());
                }
            }

            var glowWhenReadyEntities = (from part
                                        in parts
                                        let component = part.GetComponent<IShipPartComponent>()
                                        where component != null
                                        where component.Part.Flags.Contains("GlowWhenReady", StringComparer.InvariantCultureIgnoreCase)
                                        select part).ToArray();

            if (glowWhenReadyEntities.Any())
            {
                var glow = new PartGlowComponent
                {
                    Color = new Microsoft.Xna.Framework.Color(255, 255, 200)
                };
                // glow when ready
                weaponEntity.RegisterEvent(EventId.WeaponFire, (o, e) =>
                {
                    foreach (var part in glowWhenReadyEntities)
                        part.RemoveComponent<PartGlowComponent>();
                });

                weaponEntity.RegisterEvent(EventId.WeaponReady, (o, e) =>
                {
                    foreach (var part in glowWhenReadyEntities)
                        part.AddComponent(glow);
                });
            }

            shipComponent.Variant.Weapons[hardpointComponent.Hardpoint.Id] = Id;
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
                        ShipEntityFactory.GetShipModel(PartGroupId).CreateChildEntities(world, barrel);
                    }
                    break;
                case WeaponUpgradeType.Mount:
                    ShipEntityFactory.GetShipModel(PartGroupId).CreateChildEntities(world, weaponComponent.Mount);
                    break;
                case WeaponUpgradeType.Turret:
                    ShipEntityFactory.GetShipModel(PartGroupId).CreateChildEntities(world, weaponComponent.Turret);
                    break;
            }
        }
    }
}
