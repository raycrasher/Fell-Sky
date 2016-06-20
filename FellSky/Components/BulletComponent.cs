using Artemis;
using Artemis.Interface;
using FellSky.Game.Combat;
using FellSky.Game.Combat.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public class BulletComponent : IComponent, IProjectileComponent
    {
        public Color Color;
        public Entity Owner { get; set; }
        public Entity Weapon { get; set; }
        public Bullet Bullet { get; set; }

        IProjectile IProjectileComponent.Bullet => Bullet;

        public TimeSpan Age;
        public float Alpha;
    }
}
