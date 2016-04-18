using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Framework
{
    public struct FloatColor
    {
        public float R, G, B, A;

        public static implicit operator FloatColor(Color c)
        {
            return new FloatColor
            {
                R = 255f / c.R,
                G = 255f / c.G,
                B = 255f / c.B,
                A = 255f / c.A,
            };
        }

        public static implicit operator Color(FloatColor c)
        {
            return new Color((byte)(255 * c.R), (byte)(255 * c.G), (byte)(255 * c.B), (byte)(255 * c.A));
        }
    }
}
