using Microsoft.Xna.Framework;

namespace FellSky.Framework
{
    public class TimerService : ITimerService
    {
        public GameTime LastFrameRenderTime { get; set; } = new GameTime();
        public GameTime LastFrameUpdateTime { get; set; } = new GameTime();
    }
}
