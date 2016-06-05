using Artemis;
using FellSky.Components;
using FellSky.Services;
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
                //var delta = (float)_timer.DeltaTime.TotalSeconds * turret.RotateSpeed;
                turret.Rotation = turret.DesiredRotation;
            }
        }
    }
}
