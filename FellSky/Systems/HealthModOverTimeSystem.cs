using FellSky.Components;
using Artemis;
using FellSky.Services;
using Microsoft.Xna.Framework;

namespace FellSky.Systems
{
    public class HealthModOverTimeSystem : Artemis.System.EntityComponentProcessingSystem<HealthComponent, HealthModOverTimeComponent>
    {
        private ITimerService _timer;

        public HealthModOverTimeSystem(ITimerService timer)
        {
            _timer = timer;
        }

        public override void Process(Entity entity, HealthComponent health, HealthModOverTimeComponent mod)
        {
            health.Health += MathHelper.Clamp(mod.HealthDelta * (float) _timer.DeltaTime.TotalSeconds, 0, health.MaxHealth);
        }
    }
}
