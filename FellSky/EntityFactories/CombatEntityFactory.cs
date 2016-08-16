using Artemis;
using Artemis.Interface;
using FellSky.Components;
using FellSky.Game.Combat;
using FellSky.Game.Combat.Projectiles;
using FellSky.Game.Ships.Modules;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.EntityFactories
{
    public static class CombatEntityFactory
    {
        public static readonly Dictionary<string, Weapon> Weapons = new Dictionary<string, Weapon>();
        public static readonly Dictionary<string, IProjectile> Projectiles = new Dictionary<string, IProjectile>();

        public static void LoadProjectiles()
        {
            Projectiles["20mmAP"] = new Bullet
            {
                SpriteId = "bullet_boltsmall",
                Color = Color.White,
                MaxAge = TimeSpan.FromSeconds(1),
                MuzzleVelocity = 100,
                Damage = 10
            };

            Projectiles["LaserBeam"] = new Beam
            {
                Color = Color.LightYellow,
                DamagePerSecond = 20,
                Range = 800,
                SpriteId = "beam_ordinary"
            };

        }

        public static void LoadWeapons()
        {
            Weapons["20mmAutocannon"] = new Weapon
            {
                Id = "20mmAutocannon",
                Damage = 100,
                Description = "A twin-linked 20mm autocannon.",
                Name = "20mm Autocannon",
                ProjectileId = "20mmAutocannon",
                TurretModel = "Weapons/DualCannonAnimated",
                UsesFrameAnimation = true
            };

            Weapons["LaserCannon"] = new Weapon
            {
                Id = "LaserCannon",
                Name = "Laser Cannon",
                Damage = 90,
                Description = "This weapon incinerates targets using a free electron laser in the near UV range.",
                TurnRate = MathHelper.Pi / 4,
                ProjectileId = "LaserCannon",
                TurretModel = "Weapons/LaserTurret",

            };
        }
    }
    
}
