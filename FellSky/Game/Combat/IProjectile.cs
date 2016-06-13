using Artemis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Combat
{
    public interface IProjectile
    {
        Entity Spawn(EntityWorld world, Entity owner, Entity weapon, Entity muzzle);
    }
}
