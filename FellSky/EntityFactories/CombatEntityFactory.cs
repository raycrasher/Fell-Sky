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
                Scale = new Vector2(0.7f, 0.7f),
                Color = new Color(255,200,100,255),
                MaxAge = TimeSpan.FromSeconds(1),
                MuzzleVelocity = 200,
                Damage = 10
            };

            Projectiles["LaserBeam"] = new Beam
            {
                Color = Color.LightYellow,
                DamagePerSecond = 20,
                Range = 800,
                IntensityFadeInTime = 0.2f,
                IntensityFadeOutTime = 0.5f,
                FrameAnimationFps = 20,
                UseFrameAnimation = true,
                Scale = new Vector2(1, 0.4f),
                SpriteId = "crispybeam_0003_Layer-1",
                Lifetime = 0.5f
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
                ProjectileId = "20mmAP",
                TurnRate = MathHelper.Pi / 4,
                TurretModel = "Weapons/DualCannonAnimated",
                UsesFrameAnimation = true,
                AnimateWeaponCycleFps = 10,
                BurstSize = 2,
                BurstRoF = 5f,
                FireRate = 1,
            };

            Weapons["LaserCannon"] = new Weapon
            {
                Id = "LaserCannon",
                Name = "Laser Cannon",
                Damage = 90,
                Description = "This weapon incinerates targets using a free electron laser in the near UV range.",
                TurnRate = MathHelper.Pi / 2,
                FireRate = 0.5f,
                ProjectileId = "LaserBeam",
                TurretModel = "Weapons/LaserTurret",
            };

            Weapons["BoxLauncher2x2Small"] = new Weapon
            {
                Id = "BoxLauncher2x2Small",
                Name = "Box Launcher 2x2",
                Damage = 90,
                Description = "This turret-mounted 2x2 box missile launcher launches small-size missiles.",
                TurnRate = MathHelper.Pi / 10,
                ProjectileId = "SmallMissile",
                TurretModel = "Weapons/BoxLauncher2x2",
                FireRate = 1,
            };
        }
    }
    
}
