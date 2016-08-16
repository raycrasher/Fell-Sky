using Artemis;
using FellSky.Components;
using FellSky.Game.Combat;
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

                switch (weaponComponent.Status)
                {
                    case WeaponStatus.Ready:
                        if (weaponComponent.RequestFire)
                        {
                            switch (weapon.Action)
                            {
                                case WeaponAction.Automatic:
                                    if (weapon.IsMultiBarrelAlternateFire)
                                    {
                                        FireBarrel(weaponEntity, weaponComponent.Barrels[weaponComponent.CurrentBarrel], weaponComponent);
                                        weaponComponent.CurrentBarrel++;
                                        if (weaponComponent.CurrentBarrel >= weapon.NumBarrels)
                                            weaponComponent.CurrentBarrel = 0;
                                    }
                                    else
                                    {
                                        foreach (var barrel in weaponComponent.Barrels)
                                        {
                                            FireBarrel(weaponEntity, barrel, weaponComponent);
                                        }
                                    }
                                    weaponComponent.Status = WeaponStatus.Cycling;
                                    weaponComponent.CyclePercent = 0;
                                    break;
                                default:
                                    throw new NotImplementedException();
                            }
                        }
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
                    default:
                        throw new NotImplementedException();
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
            }
        }

        private void FireBarrel(Entity weaponEntity, Entity barrel, WeaponComponent weaponComponent)
        {
            // fire the projectile
            var barrelComponent = barrel.GetComponent<WeaponBarrelComponent>();      

            weaponComponent.Projectile.Spawn(EntityWorld, weaponComponent.Owner, weaponEntity, barrelComponent.Muzzle);

            // do recoil
            if (weaponComponent.Weapon.VisualRecoilMuzzleTravelDistance > 0)
            {
                barrelComponent.RecoilPercent = 0;
                barrelComponent.Status = WeaponBarrelStatus.Recoiling;
            }
        }
    }
}
