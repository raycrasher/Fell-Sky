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
using DiceNotation;

namespace FellSky.Components
{
    public class BulletComponent : ComponentPoolable
    {
        public Color Color = Color.White;
        public Entity Owner { get; set; }
        public Entity Weapon { get; set; }
        public Bullet Bullet { get; set; }

        public TimeSpan Age;
        public float Alpha = 1;
        public DiceExpression Damage;

        public override void CleanUp()
        {
            Alpha = 1;
            Age = TimeSpan.Zero;
            base.CleanUp();
        }
    }
}
