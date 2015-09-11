using FellSky.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.EntitySystems
{
    public class Camera2D
    {
        public object ScreenBounds { get; internal set; }
        public Transform Transform { get; } = new Transform();
        public float Zoom { get; set; }
    }
}
