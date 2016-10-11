using Artemis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using Microsoft.Xna.Framework;
using FellSky.Framework;
using FellSky.Game.Combat.Projectiles;
using DiceNotation;

namespace FellSky.Components
{
    public class BeamComponent : IComponent
    {
        public Color Color = Color.Cyan;
        public DiceExpression DamagePerSecond;
        public Entity Muzzle;
        public float Intensity = 0;
        public bool IsPowered = true;
        public Entity Origin;
        public Beam Beam;
        public float Range;
        public float Age; 
        public Vector2 Scale = Vector2.One;
    }
}
