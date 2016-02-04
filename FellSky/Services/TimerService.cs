using System;
using Microsoft.Xna.Framework;

namespace FellSky.Services
{
    public class TimerService : ITimerService
    {
        public TimeSpan DeltaTime => LastFrameUpdateTime.ElapsedGameTime + LastFrameRenderTime.ElapsedGameTime;

        public GameTime LastFrameRenderTime { get; set; } = new GameTime();
        public GameTime LastFrameUpdateTime { get; set; } = new GameTime();
    }
}
