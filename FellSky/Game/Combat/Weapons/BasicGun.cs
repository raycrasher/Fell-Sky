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
        public Dictionary<int,ShipPart[]> Muzzles { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public Dictionary<int, ShipPart[]> Barrels { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public ShipPart[] FixedParts { get; set; }


        /*
            Proposed scene graph structure is like this:

            Ship
                Part w/hardpoint
                    Turret mount -> transform should be Part.Transform.Matrix.Inverse, then apply position and rotation only.
                        Turret fixed entities
                        Turret rotating entity
                            Barrels (could also be flagged as fixed)
                            Mounts  (could also be flagged as fixed)
        */
        public virtual Entity Install(EntityWorld world, Entity owner, Entity hardpoint)
        {
            var turretEntity = world.CreateEntity();
            var gunComponent = new BasicGunComponent();
            gunComponent.Gun = this;
            gunComponent.Owner = owner;

            var hpXform = hardpoint.GetComponent<Transform>();
            var newXform = new Transform(hpXform.Position, hpXform.Rotation, Vector2.One);

            var matrix = Matrix.Invert(hpXform.Matrix) * newXform.Matrix;
            newXform.CopyValuesFrom(ref matrix);
            var hardpointComponent = hardpoint.GetComponent<HardpointComponent>();           
            turretEntity.AddComponent(newXform);
            turretEntity.AddComponent(hardpointComponent);
            hardpointComponent.InstalledEntity = turretEntity;
            owner.GetComponent<ShipComponent>().Turrets.Add(turretEntity);            
            turretEntity.AddComponent<IWeaponComponent>(gunComponent);
            hardpoint.GetParent().AddChild(turretEntity);

            var group = GetPartGroup(TurretId);

            Entity rotatingEntity = null;
            var muzzles = new Dictionary<int, Entity>();
            
            foreach (var part in group.Parts)
            {
                if (part.Flags.Contains("base", StringComparer.InvariantCultureIgnoreCase))
                {
                    turretEntity = part.CreateEntity(world, owner, turretEntity);
                }
                else if(part.Flags.Contains("rotating", StringComparer.InvariantCultureIgnoreCase))
                {
                    if(rotatingEntity == null)
                    {
                        rotatingEntity = world.CreateEntity();
                        turretEntity.AddChild(rotatingEntity);
                        rotatingEntity.AddComponent(new Transform());
                    }
                    rotatingEntity.AddChild(part.CreateEntity(world, owner, rotatingEntity));
                }

                var muzzleFlag = part.Flags.FirstOrDefault(f => f.StartsWith("muzzle", StringComparison.InvariantCultureIgnoreCase));
                if (muzzleFlag != null)
                {
                    int index = 0;
                    int.TryParse(muzzleFlag.Substring("muzzle".Length), out index);
                    
                    var muzzleEntity = world.CreateEntity();
                    muzzles[index] = muzzleEntity;
                    muzzleEntity.AddComponent(part.Transform);
                    if(muzzleEntity.GetParent() == null)
                    {
                        turretEntity.AddChild(muzzleEntity);
                    }
                }
            }

            gunComponent.Muzzles = muzzles.OrderBy(m => m.Key).Select(m => m.Value).ToArray();

            //group.CreateEntities(world, turretEntity);            
            //world.SpawnShipPartGroup(entity, group.PartGroup);

            if (Muzzles == null)
                Muzzles = group.GetNumberedFlaggedParts("Muzzle");
            if (Barrels == null)
                Muzzles = group.GetNumberedFlaggedParts("Barrel");

            var turret = new TurretComponent
            {
                TurnRate = TurnRate,
                FiringArc = hardpointComponent.Hardpoint.FiringArc
            };
            turretEntity.AddComponent(turret);

            // todo: set projectile

            return turretEntity;
        }

        

        public virtual void Fire(EntityWorld world, Entity owner, Entity weapon)
        {
            var weaponComponent = weapon.GetComponent<IWeaponComponent>();
            
            
        }

        public virtual void Uninstall(Entity owner, Entity weaponEntity)
        {
            owner.GetComponent<ShipComponent>().Turrets.Remove(weaponEntity);
            weaponEntity.GetComponent<HardpointComponent>().InstalledEntity = null;
            weaponEntity.DeleteWithChildren();
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
