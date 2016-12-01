using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;

namespace FellSky.Game.Combat.Projectiles
{
    public class Missile : IProjectile
    {
        public float BlastRadius { get; set; }
        public string Damage { get; set; }
        public DamageType DamageType { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Role { get; internal set; }
        public float SeekAngle { get; set; }
        public string SpriteId { get; set; }
        public string TrailEffectId { get; set; }


        public Entity Spawn(EntityWorld world, Entity owner, Entity weapon, Entity muzzle)
        {
            throw new NotImplementedException();
        }
    }
}
