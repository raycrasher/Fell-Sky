using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky
{
    public static class Constants
    {
        public static readonly DateTime EpochJ2000 = new DateTime(2000, 1, 1);
        public const double StarSystemScale = 1e-5;
        public const double GravitationalConstant = 6.674e-11d;
        public const long TicksPerFrame = 17; // 1 tick = 1 ms
        public const float PhysicsUnitScale = 0.01f;
        public const string ActiveCameraTag = "ActiveCamera";
    }
}
