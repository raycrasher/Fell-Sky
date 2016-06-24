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
        public ShipPart[] Muzzles { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public Dictionary<int, ShipPart[]> Barrels { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public ShipPart[] FixedParts { get; set; }


        /*
            Proposed scene graph structure is like this:

            Ship
                Part w/hardpoint
                    Weapon mount -> transform should be Part.Transform.Matrix.Inverse, then apply position and rotation only.
                        Turret fixed entities
                        Turret rotating entity
                            Barrels (could also be flagged as fixed)
                            Mounts  (could also be flagged as fixed)
        */
        public virtual Entity Install(EntityWorld world, Entity ownerEntity, Entity hardpointEntity)
        {
            var hardpointComponent = hardpointEntity.GetComponent<HardpointComponent>();                     
            var hardpointTransform = hardpointEntity.GetComponent<Transform>();
            var group = GetPartGroup(TurretId);

            // create weapon entity
            var weaponEntity = world.CreateEntity();
            var gunComponent = new BasicGunComponent
            {
                Gun = this,
                Owner = ownerEntity
            };
            weaponEntity.AddComponent<IWeaponComponent>(gunComponent);

            // set weapon transform

            var weaponTransform = new Transform(Vector2.Zero, 0, Vector2.One, -hardpointTransform.Origin);
            
            var scale = hardpointTransform.Scale;
            var origin = -hardpointTransform.Origin;
            if (scale.X < 0)
            {
                scale.X = Math.Abs(scale.X);
                origin.X *= -1;
            }
            if (scale.Y < 0)
            {
                scale.Y *= Math.Abs(scale.Y);
                origin.Y *= -1;
            }
            weaponTransform.Scale = scale;
            weaponTransform.Origin = origin;


            weaponEntity.AddComponent(weaponTransform);
            weaponEntity.AddComponent(hardpointComponent);
            hardpointComponent.InstalledEntity = weaponEntity;

            // create turret entity;
            var turretEntity = world.CreateEntity();
            var turretComponent = new TurretComponent
            {
                TurnRate = TurnRate,
                FiringArc = hardpointComponent.Hardpoint.FiringArc
            };
            turretEntity.AddComponent(turretComponent);
            turretEntity.AddComponent(new Transform());
            turretComponent.HardpointEntity = hardpointEntity;
            gunComponent.Turret = turretEntity;
            ownerEntity.GetComponent<ShipComponent>().Turrets.Add(turretEntity);

            var muzzles = new Dictionary<int, Entity>();

            foreach (var part in group.Parts)
            {
                if (part.Flags == null || part.Flags.Contains("turret", StringComparer.InvariantCultureIgnoreCase))
                {
                    part.CreateEntity(world, ownerEntity, turretEntity);
                }
                else if (part.Flags.Contains("base", StringComparer.InvariantCultureIgnoreCase))
                {
                    part.CreateEntity(world, ownerEntity, weaponEntity);
                }

                var muzzleFlag = part.Flags?.FirstOrDefault(f => f.StartsWith("muzzle", StringComparison.InvariantCultureIgnoreCase));
                if (muzzleFlag != null)
                {
                    int index = 0;
                    int.TryParse(muzzleFlag.Substring("muzzle".Length), out index);
                    
                    var muzzleEntity = world.CreateEntity();
                    muzzles[index] = muzzleEntity;
                    muzzleEntity.AddComponent(part.Transform.Clone());
                    if(muzzleEntity.GetParent() == null)
                    {
                        turretEntity.AddChild(muzzleEntity);
                    }
                }
            }

            // set parenting
            hardpointEntity.AddChild(weaponEntity);
            weaponEntity.AddChild(turretEntity);


            gunComponent.Muzzles = muzzles.OrderBy(m => m.Key).Select(m => m.Value).ToArray();

            return turretEntity;
        }

        

        public virtual void Fire(EntityWorld world, Entity owner, Entity weapon)
        {
            var weaponComponent = (BasicGunComponent)weapon.GetComponent<IWeaponComponent>();
            Projectile = Projectile ?? WeaponEntityFactory.Projectile[ProjectileId];

            foreach (var muzzle in weaponComponent.Muzzles)
                Projectile.Spawn(world, owner, weapon, muzzle);
            
        }

        public virtual void Uninstall(Entity owner, Entity weaponEntity)
        {
            var turret = ((BasicGunComponent)weaponEntity.GetComponent<IWeaponComponent>()).Turret;
            owner.GetComponent<ShipComponent>().Turrets.Remove(turret);
            owner.GetComponent<ShipComponent>().Weapons.Remove(weaponEntity);
            weaponEntity.GetComponent<HardpointComponent>().InstalledEntity = null;
            weaponEntity.DeleteFromSceneGraph();
        }

        private static ShipPartGroup GetPartGroup(string id)
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
            return group;
        }

        private static readonly Dictionary<string, ShipPartGroup> PartGroups = new Dictionary<string, ShipPartGroup>();
    }
}
