using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using FellSky.Components;
using Microsoft.Xna.Framework;
using System.ComponentModel;
using FellSky.Framework;
using FellSky.Services;

namespace FellSky.Game.Combat.Projectiles
{
    public class Beam : IProjectile
    {
        public float Range { get; set; }
        public float DamagePerSecond { get; set; }
        public string SpriteId { get; set; }
        public Color Color { get; set; }

        [Newtonsoft.Json.JsonIgnore, Browsable(false)]
        public Sprite Sprite;
        private ISpriteManagerService _spriteManager;

        public Entity Spawn(EntityWorld world, Entity owner, Entity weapon, Entity muzzle)
        {
            var beamEntity = world.CreateEntity();
            var weaponComponent = weapon.GetComponent<WeaponComponent>();
            _spriteManager = _spriteManager ?? ServiceLocator.Instance.GetService<ISpriteManagerService>();
            var sprite = _spriteManager.CreateSpriteComponent(SpriteId);

            Matrix matrix;
            muzzle.GetWorldMatrix(out matrix);
            var beamTransform = new Transform();
            beamTransform.CopyValuesFrom(ref matrix);
            beamEntity.AddComponent(beamTransform);
            beamEntity.AddComponent(sprite);
            beamEntity.AddComponent(new BeamComponent {
                Muzzle = muzzle,
                DamagePerSecond = DamagePerSecond + weaponComponent.Weapon.DamagePerSecond,
                Color = Color,
                Origin = muzzle,
                Range = Range
            });

            return beamEntity;
        }
    }
}
