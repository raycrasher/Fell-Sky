using Artemis;
using FellSky.Components;
using FellSky.Game.Combat;
using FellSky.Game.Combat.Projectiles;
using FellSky.Services;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Systems
{
    public class WeaponSystem: Artemis.System.EntitySystem
    {
        WeaponFireEventArgs _weaponFireEventArgs = new WeaponFireEventArgs();
        private ITimerService _timer;

        public WeaponSystem()
            : base(Aspect.All(typeof(WeaponComponent)))
        {
            _timer = ServiceLocator.Instance.GetService<ITimerService>();
        }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            var delta = (entityWorld.Delta / 1000f);

            foreach (var weaponEntity in entities.Values)
            {
                var weaponComponent = weaponEntity.GetComponent<WeaponComponent>();
                var weapon = weaponComponent.Weapon;

                var lastStatus = weaponComponent.Status;

                switch (weaponComponent.Status)
                {
                    case WeaponStatus.Ready:
                        if(weaponComponent.LastStatus != WeaponStatus.Ready)
                            weaponEntity.FireEvent(this, EventId.WeaponReady, null);

                        if (weaponComponent.RequestFire)
                        {
                            switch (weapon.Action)
                            {
                                case WeaponAction.Automatic:
                                    weaponEntity.FireEvent(this, EventId.WeaponFire, _weaponFireEventArgs);

                                    if (weapon.BurstSize > 1)
                                    {
                                        weaponComponent.Status = WeaponStatus.BurstCycling;
                                        weaponComponent.CyclePercent = 0;

                                        FireBarrel(weaponEntity, 0, weaponComponent);

                                        if (weapon.NeedsAmmo && weaponComponent.AmmoLeft <= 0)
                                        {
                                            weaponComponent.Status = WeaponStatus.Reloading;
                                            weaponComponent.ReloadPercent = 0;
                                            break;
                                        }
                                    }
                                    else {
                                        weaponComponent.Status = WeaponStatus.Cycling;
                                        weaponComponent.CyclePercent = 0;
                                        for (int index = 0; index < weaponComponent.Barrels.Length; index++)
                                        {
                                            FireBarrel(weaponEntity, index, weaponComponent);
                                            if (weapon.NeedsAmmo && weaponComponent.AmmoLeft <= 0)
                                            {
                                                weaponComponent.Status = WeaponStatus.Reloading;
                                                weaponComponent.ReloadPercent = 0;
                                                break;
                                            }
                                        }                                        
                                    }
                                    break;
                                case WeaponAction.ContinuousFire:
                                    for (int index = 0; index < weaponComponent.Barrels.Length; index++)
                                    {
                                        FireBarrel(weaponEntity, index, weaponComponent);
                                        if (weapon.NeedsAmmo && weaponComponent.AmmoLeft <= 0)
                                        {
                                            weaponComponent.Status = WeaponStatus.Reloading;
                                            weaponComponent.ReloadPercent = 0;
                                            break;
                                        }
                                    }
                                    weaponComponent.Status = WeaponStatus.ContinuousFiring;
                                    break;
                                default:
                                    throw new NotImplementedException();
                            }
                        }
                        break;
                    case WeaponStatus.ContinuousFiring:
                        if (!weaponComponent.RequestFire)
                        {
                            weaponComponent.Status = WeaponStatus.Cycling;
                            if(weaponComponent.Projectile is Beam)
                            {
                                foreach(var muzzle in weaponComponent.Barrels.Select(b => b.GetComponent<WeaponBarrelComponent>().Muzzle))
                                {
                                    muzzle.GetComponent<BeamEmitterComponent>().BeamEntity.GetComponent<BeamComponent>().IsPowered = false;
                                }
                            }
                        }
                        break;
                    case WeaponStatus.BurstCycling:
                        if (weaponComponent.CyclePercent >= 1)
                        {
                            weaponComponent.CyclePercent = 1;
                            FireBarrel(weaponEntity, weaponComponent.CurrentBarrel, weaponComponent);
                            if (weapon.NeedsAmmo && weaponComponent.AmmoLeft <= 0)
                            {
                                weaponComponent.Status = WeaponStatus.Reloading;
                                weaponComponent.ReloadPercent = 0;
                                break;
                            }
                            else {
                                if (weaponComponent.CurrentBarrel == 0)
                                {
                                    weaponComponent.Status = WeaponStatus.Cycling;
                                    weaponComponent.CyclePercent = 0;
                                }
                                else
                                {
                                    weaponComponent.Status = WeaponStatus.BurstCycling;
                                    weaponComponent.CyclePercent = 0;
                                }
                            }
                        }
                        else weaponComponent.CyclePercent += weapon.BurstRoF * delta;
                        break;
                    case WeaponStatus.Cycling:                    
                        if (weaponComponent.CyclePercent >= 1) {
                            weaponComponent.CyclePercent = 1;
                            weaponComponent.Status = WeaponStatus.Ready;
                        }
                        else weaponComponent.CyclePercent += weapon.FireRate * delta;
                        break;
                    case WeaponStatus.Reloading:
                        if (weaponComponent.ReloadPercent >= 1)
                        {
                            weaponComponent.ReloadPercent = 1;
                            weaponComponent.Status = WeaponStatus.Cycling;
                            weaponComponent.CyclePercent = 0;
                        }
                        else weaponComponent.CyclePercent += weapon.FireRate * delta;
                        break;
                    case WeaponStatus.Disabled:
                        break;
                }

                // handle barrel recoil
                if (weaponComponent.Status != WeaponStatus.Disabled)
                {
                    foreach(var barrelEntity in weaponComponent.Barrels)
                    {
                        var barrelComponent = barrelEntity.GetComponent<WeaponBarrelComponent>();
                        var barrelXform = barrelEntity.GetComponent<Transform>();

                        if (barrelComponent.Status != WeaponBarrelStatus.Idle)
                        {
                            Vector2 pos = Vector2.Zero;
                            pos.X = MathHelper.Lerp(0, -weapon.VisualRecoilMuzzleTravelDistance, barrelComponent.RecoilPercent);
                            barrelXform.Position = pos;
                        }

                        switch (barrelComponent.Status)
                        {
                            case WeaponBarrelStatus.Recoiling:
                                if (barrelComponent.RecoilPercent >= 1)
                                {
                                    barrelComponent.RecoilPercent = 1;
                                    barrelComponent.Status = WeaponBarrelStatus.Cycling;
                                }
                                else barrelComponent.RecoilPercent += delta * weapon.VisualRecoilSpeed;
                                break;
                            case WeaponBarrelStatus.Cycling:
                                if (barrelComponent.RecoilPercent < 0)
                                {
                                    barrelComponent.RecoilPercent = 0;
                                    barrelComponent.Status = WeaponBarrelStatus.Idle;
                                    barrelXform.Position = Vector2.Zero;
                                }
                                else barrelComponent.RecoilPercent -= delta * weapon.VisualRecoilCycleSpeed;
                                break;
                            case WeaponBarrelStatus.Idle:
                                break;
                        }
                    }
                }
                weaponComponent.LastStatus = lastStatus;
            }
        }

        public override void OnAdded(Entity entity)
        {
            var cmp = entity.GetComponent<WeaponComponent>();
        }

        private void FireBarrel(Entity weaponEntity, int barrelIndex, WeaponComponent weaponComponent)
        {
            // fire the projectile
            var barrel = weaponComponent.Barrels[barrelIndex];
            var barrelComponent = barrel.GetComponent<WeaponBarrelComponent>();      
            weaponComponent.Projectile.Spawn(EntityWorld, weaponComponent.Owner, weaponEntity, barrelComponent.Muzzle);
            weaponComponent.AmmoLeft--;

            weaponComponent.CurrentBarrel++;
            if (weaponComponent.CurrentBarrel >= weaponComponent.Barrels.Length)
                weaponComponent.CurrentBarrel = 0;

            // do recoil
            if (weaponComponent.Weapon.VisualRecoilMuzzleTravelDistance > 0)
            {
                barrelComponent.RecoilPercent = 0;
                barrelComponent.Status = WeaponBarrelStatus.Recoiling;
            }
        }
    }
}
