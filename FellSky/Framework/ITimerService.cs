using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Framework
{
    public interface ITimerService
    {
        GameTime LastFrameRenderTime { get; }
        GameTime LastFrameUpdateTime { get; }
    }
}
