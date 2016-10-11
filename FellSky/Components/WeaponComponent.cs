using Artemis;
using Artemis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FellSky.Game.Ships.Modules;
using FellSky.Game.Combat;
using DiceNotation;

namespace FellSky.Components
{
    class WeaponComponent: IComponent
    {
        public Entity Mount;
        public Entity[] Barrels;
        public Entity Turret;
        public IProjectile Projectile = null;
        public Weapon Weapon;
        public int CurrentBarrel;
        public bool RequestFire = false;
        // TODO: Add fire control
        public WeaponStatus Status;
        public float CyclePercent;
        public float ReloadPercent;
        public Entity Owner;
        public int AmmoInMagazine;
        public int AmmoLeft;

        public WeaponStatus LastStatus;
        //public EventHandler<WeaponFireEventArgs> OnFire;
    }
}
