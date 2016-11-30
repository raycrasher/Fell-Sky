using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky
{
    public static class EventId
    {
        public const int WeaponFire = 100;
        public const int WeaponFireProjectile = 101;
        public const int WeaponReady = 102;

        public const int BeamHit = 200;
        public const int BulletHit = 201;

        public const int OnDamage = 250;

        public const int PhysicsCollision = 300;
        public const int PhysicsOnSeparation = 301;
        public const int PhysicsAfterSeparation = 302;
        public const int AfterCollision = 303;
        public const int BeforeCollision = 304;
    }
}
