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
        private Dictionary<Entity, Coroutine> _coroutines = new Dictionary<Entity, Coroutine>();

        public CoroutineSystem(ITimerService timer)
            : base(Aspect.All(typeof(CoroutineComponent)))
        {
            _timer = timer;
        }

        public override void OnAdded(Entity entity)
        {
            var component = entity.GetComponent<CoroutineComponent>();
            component.Coroutine = _service.StartCoroutine(component.CoroutineFunction);
            component.Coroutine.OnDone = component.OnDone;
            _coroutines[entity] = component.Coroutine;
            base.OnAdded(entity);
        }

        public override void OnRemoved(Entity entity)
        {
            var coroutine = _coroutines[entity];
            coroutine.Stop();
            _coroutines.Remove(entity);
            base.OnRemoved(entity);
        }

        public override void Process()
        {
            _service.RunCoroutines(_timer.DeltaTime);
        }
    }
}
