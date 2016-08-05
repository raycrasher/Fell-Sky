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
    class WeaponBarrelComponent: IComponent
    {
        public Entity Muzzle;
        public WeaponBarrelStatus Status;
        public float RecoilPercent = 0;
    }
}
