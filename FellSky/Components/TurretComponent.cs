using Artemis;
using Artemis.Interface;
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
        public float LimitAngle = MathHelper.ToRadians(120);
        public float TurnRate = MathHelper.TwoPi;
        public bool IsEnabled = true;

        public bool IsOmniTurret => LimitAngle > MathHelper.TwoPi;
    }
}
