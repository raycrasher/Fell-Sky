using Artemis.Interface;
using FellSky.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Graphics
{
    public class Camera2D: IComponent
    {
        public Rectangle ScreenBounds { get; internal set; }
        public Transform Transform { get; } = new Transform();
        public float Zoom { get; set; }
        public Matrix CurrentMatrix { get; private set; }
    }
}
