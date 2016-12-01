using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using FellSky.Services;
using Microsoft.Xna.Framework;
using FellSky.Systems;
using FellSky.Components;
using DiceNotation;

namespace FellSky.Game.Combat.Projectiles
{
    public class Bullet : IProjectile
    {
        private ISpriteManagerService _spriteManager;

        public float MuzzleVelocity { get; set; } = 200;
        public string SpriteId { get; set; }
        public Color Color { get; set; } = Color.White;
        public TimeSpan MaxAge { get; set; } = TimeSpan.FromSeconds(1);
        
        public Vector2 Scale { get; set; } = Vector2.One;

        public string Damage {
            get { return _damage; }
            set {
                _damage = value;
                _damageDice = Dice.Parse(value);
            }
        }

        public DamageType DamageType { get; set; } = DamageType.Kinetic;

        private string _damage;
        private DiceExpression _damageDice;

        public Entity Spawn(EntityWorld world, Entity owner, Entity weapon, Entity muzzle)
        {
            _spriteManager = _spriteManager ?? ServiceLocator.Instance.GetService<ISpriteManagerService>();
            var bulletEntity = world.CreateEntity();
            var sprite = _spriteManager.CreateSpriteComponent(SpriteId);
            bulletEntity.AddComponent(sprite);
            var xform = bulletEntity.AddComponentFromPool<Transform>();
            Matrix matrix;
            muzzle.GetWorldMatrix(out matrix);
            xform.CopyValuesFrom(ref matrix);
            xform.Scale = Scale;
            xform.Origin = sprite.Origin;

            var radius = Math.Min(sprite.TextureRect.Width/2, sprite.TextureRect.Height/2) * Constants.PhysicsUnitScale;
            var physics = world.SystemManager.GetSystem<PhysicsSystem>();
            var rigidBody = physics.CreateRigidBody(xform.Position, xform.Rotation);
            rigidBody.Body.IgnoreCollisionWith(owner.GetComponent<RigidBodyComponent>().Body);
            rigidBody.Body.IsStatic = false;
            FarseerPhysics.Factories.FixtureFactory.AttachCircle(radius, 0.01f, rigidBody.Body, bulletEntity);
            rigidBody.Body.LinearVelocity = Utilities.CreateVector2FromAngle(xform.Rotation) * MuzzleVelocity * Constants.PhysicsUnitScale;
            bulletEntity.AddComponent(rigidBody);
            rigidBody.Body.UserData = bulletEntity;
            var fixture = rigidBody.Body.FixtureList[0];
            var iff = owner.GetComponent<IFFComponent>();
            bulletEntity.AddComponent(iff);

            fixture.BeforeCollision += (a, b) =>
            {
                var entity = b.UserData as Entity;
                if (entity.HasComponent<IFFComponent>() && entity.GetComponent<IFFComponent>().IffCode == iff.IffCode)
                    return false;
                
                return true;
            };

            var bulletComponent = bulletEntity.AddComponentFromPool<BulletComponent>();
            bulletComponent.Owner = owner;
            bulletComponent.Weapon = weapon;
            bulletComponent.Bullet = this;
            bulletComponent.Color = Color;
            bulletComponent.Age = TimeSpan.Zero;
            bulletComponent.Alpha = 1;
            bulletComponent.Damage = _damageDice;            
            //bulletEntity.AddComponent<IProjectileComponent>(bulletComponent);

            return bulletEntity;
        }
    }
}
