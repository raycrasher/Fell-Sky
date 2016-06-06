using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Combat
{
    public class BasicBullet: IProjectile
    {
        public TimeSpan Lifetime;
        public float MuzzleVelocity;
        public float HitRadius;
        public float Damage;        // for now, a simplistic damage model will do. This may be replaced in further development.
        public string SpriteId;
        public Color Color;
    }
}
