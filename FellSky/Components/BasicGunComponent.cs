using Artemis.Interface;
using FellSky.Game.Combat.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using FellSky.Game.Combat;

namespace FellSky.Components
{
    public class BasicGunComponent: IWeaponComponent
    {
        public BasicGun Gun;
        public Entity Owner;
        public IProjectile Projectile;
        public Entity[] Muzzles;
        public Entity[][] Barrels;
        public Entity Turret;

        public TimeSpan FireTimer;
        public TimeSpan RecoilTimer;
        public TimeSpan ReloadTimer;

        public bool NoRecoilAnimation;

        public int CurrentBarrel;
        public int NumBarrels;

        public bool RequestFire { get; set; }
        public WeaponStatus Status { get; set; }

        IWeapon IWeaponComponent.Weapon => Gun;
    }
}
