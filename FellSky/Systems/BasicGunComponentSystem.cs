using Artemis;
using FellSky.Components;
using FellSky.Game.Combat.Weapons;
using FellSky.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Systems
{
    public class BasicGunComponentSystem: Artemis.System.EntitySystem
    {
        private ITimerService _timer;

        public BasicGunComponentSystem()
            : base(Aspect.All(typeof(BasicGunComponent)))
        {
            _timer = ServiceLocator.Instance.GetService<ITimerService>();
        }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            foreach(var entity in entities.Values)
            {
                var gunComponent = entity.GetComponent<BasicGunComponent>();
                switch (gunComponent.Status)
                {
                    case WeaponStatus.Idle:
                        if (gunComponent.RequestFire)
                            gunComponent.Status = WeaponStatus.Firing;
                        break;
                    case WeaponStatus.Firing:
                        gunComponent.Gun.Fire(EntityWorld, gunComponent.Owner, entity);
                        gunComponent.Status = WeaponStatus.Recoiling;
                        break;
                    case WeaponStatus.Recoiling:
                        break;
                    case WeaponStatus.RecoilReset:
                        break;
                    case WeaponStatus.Cycling:
                        break;
                    case WeaponStatus.Reloading:
                        break;
                    case WeaponStatus.OutOfAmmo:
                        break;
                    case WeaponStatus.Disabled:
                        break;
                }
            }
        }

    }
}
