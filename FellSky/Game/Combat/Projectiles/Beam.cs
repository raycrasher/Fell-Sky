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
using DiceNotation;

namespace FellSky.Game.Combat.Projectiles
{
    public class Beam : IProjectile
    {
        public float Range { get; set; }
        
        public string SpriteId { get; set; }
        public Color Color { get; set; }
        public bool UseFrameAnimation { get; set; }
        public float FrameAnimationFps { get; set; } = 20f;
        public float IntensityFadeInTime { get; set; } = 0.2f;
        public float IntensityFadeOutTime { get; set; } = 0.3f;
        public float Lifetime { get; set; } = 0;

        public string DamagePerSecond
        {
            get { return _damage; }
            set
            {
                _damage = value;
                _damageDice = Dice.Parse(value);
            }
        }
        private string _damage;
        private DiceExpression _damageDice;

        public Vector2 Scale { get; set; } = Vector2.One;

        [Newtonsoft.Json.JsonIgnore, Browsable(false)]
        public Sprite Sprite;

        [Newtonsoft.Json.JsonIgnore, Browsable(false)]
        private ISpriteManagerService _spriteManager;

        public Entity Spawn(EntityWorld world, Entity owner, Entity weapon, Entity muzzle)
        {
            var beamEntity = world.CreateEntity();
            var weaponComponent = weapon.GetComponent<WeaponComponent>();
            _spriteManager = _spriteManager ?? ServiceLocator.Instance.GetService<ISpriteManagerService>();
            var sprite = _spriteManager.CreateSpriteComponent(SpriteId);

            var emitter = muzzle.GetComponent<BeamEmitterComponent>();
            if (emitter!=null)
                emitter.BeamEntity = beamEntity;
            
            Matrix matrix;
            muzzle.GetWorldMatrix(out matrix);
            var beamTransform = beamEntity.AddComponentFromPool<Transform>();
            beamTransform.CopyValuesFrom(ref matrix);
            beamEntity.AddComponent(sprite);
            beamEntity.AddComponent(new BeamComponent {
                Beam = this,
                Muzzle = muzzle,
                DamagePerSecond = _damageDice,
                Color = Color,
                Origin = muzzle,
                Range = Range,
                Scale = Scale
            });

            if (UseFrameAnimation)
            {
                var animComponent = _spriteManager.CreateFrameAnimationComponent(SpriteId, FrameAnimationFps);
                beamEntity.AddComponent(animComponent);
                animComponent.Play();
                animComponent.Loop = true;
            }

            return beamEntity;
        }
    }
}
