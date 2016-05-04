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
    public class CoroutineSystem: Artemis.System.EntitySystem
    {
        CoroutineService _service = new CoroutineService();
        private ITimerService _timer;

        public CoroutineSystem(ITimerService timer)
            : base(Aspect.All(typeof(CoroutineComponent)))
        {
            _timer = timer;
        }

        public override void OnAdded(Entity entity)
        {
            var component = entity.GetComponent<CoroutineComponent>();
            component.Coroutine = _service.StartCoroutine(component.CoroutineFunction);
            base.OnAdded(entity);
        }

        public override void OnRemoved(Entity entity)
        {
            var component = entity.GetComponent<CoroutineComponent>();
            component.Coroutine.Stop();
            base.OnRemoved(entity);
        }

        public override void Process()
        {
            _service.RunCoroutines(_timer.DeltaTime);
        }
    }
}
