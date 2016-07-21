using Artemis;
using Artemis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FellSky.Game.Ships.Modules;

namespace FellSky.Components
{
    class WeaponComponent: IComponent
    {
        public Entity Base;
        public Entity[] Barrels;
        public Entity Turret;

        public Weapon Weapon;
    }
}
