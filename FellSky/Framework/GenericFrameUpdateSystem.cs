using Artemis;
using Artemis.Attributes;

namespace FellSky.Framework
{
    [ArtemisEntitySystem(ExecutionType = Artemis.Manager.ExecutionType.Synchronous, GameLoopType = Artemis.Manager.GameLoopType.Update, Layer = -1)]
    public class GenericFrameUpdateSystem : Artemis.System.EntityComponentProcessingSystem<FrameUpdateComponent>
    {
        private ITimerService _timer;

        protected override void Begin()
        {
            _timer = BlackBoard.GetService<ITimerService>();
            base.Begin();
        }
        public override void Process(Entity entity, FrameUpdateComponent update)
        {
            update.UpdateFunction?.Invoke(_timer?.LastFrameUpdateTime, entity);
        }
    }
}
