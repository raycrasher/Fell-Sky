using Artemis;
using Artemis.Interface;
using FellSky.Game.Combat;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public class BasicBulletComponent : IComponent
    {
        public Color Color;
        public Entity Owner;
        public Entity FiringWeapon;
        public BasicBullet Bullet;
        public TimeSpan Age;
        public float Alpha;
    }
}
