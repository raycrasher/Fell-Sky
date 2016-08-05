using Artemis;
using Artemis.Interface;
using FellSky.Game.Ships.Parts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public class TurretComponent: IComponent
    {
        public float Rotation = 0;
        public float DesiredRotation = 0;
        public float FiringArc = MathHelper.ToRadians(120);
        public float TurnRate = MathHelper.TwoPi;
        public bool IsEnabled = true;

        public float AimTolerance = 0.1f;
        public bool IsAimed => Math.Abs(Utilities.GetLesserAngleDifference(Rotation, DesiredRotation)) <= AimTolerance;
        public Entity Weapon;
        public Entity Ship;

        public bool IsOmnidirectional => FiringArc >= MathHelper.TwoPi;
        public bool IsFixedTurret => FiringArc <= 0;
    }
}
