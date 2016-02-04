using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Services
{
    public interface IKeyboardService
    {
        event Action<Keys> KeyDown, KeyUp;
        bool IsKeyDown(Keys UpKey);
    }
}
