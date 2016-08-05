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

namespace FellSky.Game.Combat.Projectiles
{
    public class Bullet : IProjectile
    {
        private ISpriteManagerService _spriteManager;
        public float MuzzleVelocity { get; set; } = 100;
        public string SpriteId { get; set; }
        public Color Color { get; set; }
        public TimeSpan MaxAge { get; set; }
        public float Damage { get; set; }

        public Entity Spawn(EntityWorld world, Entity owner, Entity weapon, Entity muzzle)
        {
            _spriteManager = _spriteManager ?? ServiceLocator.Instance.GetService<ISpriteManagerService>();
            var bulletEntity = world.CreateEntity();
            var sprite = _spriteManager.CreateSpriteComponent(SpriteId);
            bulletEntity.AddComponent(sprite);
            var xform = new Transform();
            Matrix matrix;
            muzzle.GetWorldMatrix(out matrix);
            xform.CopyValuesFrom(ref matrix);
            bulletEntity.AddComponent(xform);

            var radius = Math.Min(sprite.TextureRect.Width/2, sprite.TextureRect.Height/2);
            var physics = world.SystemManager.GetSystem<PhysicsSystem>();
            var rigidBody = physics.CreateRigidBody(xform.Position, xform.Rotation);
            FarseerPhysics.Factories.FixtureFactory.AttachCircle(radius, 0.01f, rigidBody.Body, bulletEntity);
            rigidBody.Body.ApplyForce(rigidBody.Body.GetWorldVector(Utilities.CreateVector2FromAngle(xform.Rotation)) * MuzzleVelocity);

            var bulletComponent = new BulletComponent()
            {
                Owner = owner,
                Weapon = weapon,
                Bullet = this,
                Color = Color,
                Age = TimeSpan.Zero,
                Alpha = 1
            };

            bulletEntity.AddComponent<IProjectileComponent>(bulletComponent);

            return bulletEntity;
        }
    }
}
