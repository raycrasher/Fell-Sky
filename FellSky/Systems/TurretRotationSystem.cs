using Artemis;
using FellSky.Components;
using FellSky.Services;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Systems
{
    public class TurretRotationSystem: Artemis.System.EntityComponentProcessingSystem<TurretComponent>
    {
        private ITimerService _timer;

        public TurretRotationSystem()
        {
            _timer = ServiceLocator.Instance.GetService<ITimerService>();
        }
        public override void Process(Entity entity, TurretComponent turret)
        {
            if (turret.IsEnabled)
            {
                var speed = (float)_timer.DeltaTime.TotalSeconds * turret.TurnRate;
                float deltaAngle = 0;

                if (turret.IsOmnidirectional)
                {
                    var a = MathHelper.WrapAngle(turret.DesiredRotation - turret.Rotation);
                    deltaAngle = MathHelper.Clamp(a, -speed, speed);
                    turret.Rotation += deltaAngle;
                }
                else
                {
                    var a = MathHelper.WrapAngle(turret.DesiredRotation - turret.Rotation);
                    var b = MathHelper.WrapAngle(turret.Rotation);
                    var c = MathHelper.WrapAngle(turret.DesiredRotation);

                    if (Math.Sign(b) == Math.Sign(c))
                    {
                        deltaAngle = MathHelper.Clamp(a, -speed, speed);
                    }
                    else {
                        if (turret.Rotation + a > Math.PI || turret.Rotation + a < -Math.PI)
                        {
                            deltaAngle = MathHelper.Clamp(-a, -speed, speed);
                        } else
                        {
                            deltaAngle = MathHelper.Clamp(a, -speed, speed);
                        }
                    }
                    turret.Rotation = MathHelper.Clamp(MathHelper.WrapAngle(turret.Rotation + deltaAngle), -turret.FiringArc/2, turret.FiringArc/2);
                }
            }
            entity.GetComponent<Transform>().Rotation = turret.Rotation;
        }
    }
}
