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

        IWeapon IWeaponComponent.Weapon => Gun;
    }
}
