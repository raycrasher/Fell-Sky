using Artemis;
using Artemis.Interface;
using FellSky.Game.Combat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public interface IProjectileComponent: IComponent
    {
        Entity Owner { get; }
        Entity Weapon { get; }
        IProjectile Bullet { get; }
    }
}
