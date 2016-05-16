using Artemis;
using Artemis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FellSky.Game.Combat.Weapons;

namespace FellSky.Components
{
    public class WeaponComponent: IComponent
    {
        public Func<Entity> Fire;
        public WeaponData Data;
    }
}
