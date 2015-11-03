using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Framework
{
    public interface IMouseService
    {
        event Action<Point, int> ButtonDown, ButtonUp;
        event Action<Point> Move;
        event Action<int> WheelChanged;
        Vector2 ScreenPosition { get; set; }
    }
}
