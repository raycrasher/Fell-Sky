using Artemis;
using Artemis.Interface;
using FellSky.Components;
using FellSky.Game.Combat;
using FellSky.Game.Combat.Weapons;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.EntityFactories
{
    public static class WeaponEntityFactory
    {
        public static readonly Dictionary<string, IWeapon> Weapons = new Dictionary<string, IWeapon>();

        public static void LoadWeapons()
        {
            Weapons["20mmAutocannon"] = new BasicGun
            {
                Id = "20mmAutocannon",
                DamagePerSecond = 100,
                Description = "A twin-linked 20mm autocannon.",
                Name = "20mm Autocannon",
                ProjectileId = "20mmAutocannon",
                TurretId = "DualMG"
            };

            Weapons["LaserCannon"] = new BasicGun
            {
                Id = "LaserCannon",
                Name = "Laser Cannon",
                DamagePerSecond = 90,
                Description = "This weapon incinerates targets using a free electron laser in the near UV range.",
                TurnRate = MathHelper.Pi / 4,
                ProjectileId = "LaserCannon",
                TurretId = "LaserCannon"
            };
        }
    }
    
}
