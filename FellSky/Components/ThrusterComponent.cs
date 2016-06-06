using Artemis;
using Artemis.Interface;
using FellSky.Framework;
using FellSky.Game.Ships.Parts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public class ThrusterComponent: ShipPartComponent<Thruster>
    {
        public ThrusterComponent(Thruster part, Entity ship)
            : base(part, ship)
        {
        }

        public float GetAngularThrustMult(AngularDirection dir, Vector2 centerMass)
        {
            var offset = Part.Transform.Position - centerMass;
            if (float.IsNaN(offset.X)) return 0;
            var r = ((float)dir * Math.Sign(offset.X)) * Part.Transform.Rotation;

            var a = Math.Abs(Utilities.GetLesserAngleDifference(MathHelper.Pi / 2, r));

            const float AngleCutoff = 0.1f;

            if (a < AngleCutoff)
            {
                return AngleCutoff / (1 - a / AngleCutoff);
            }
            return 0;
        }

        public bool IsThrusting;
        public float ThrustPercentage;
    }
}
