using Artemis;
using Artemis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FellSky.Game.Combat.Weapons;
using FellSky.Game.Combat;

namespace FellSky.Components
{
    public interface IWeaponComponent: IComponent
    {
        IWeapon Weapon { get; }
    }
}
