using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky
{
    public struct ColorHSL
    {
        public float Hue, Saturation, Luminosity;
        public byte Alpha;

        public ColorHSL(float h, float s, float l, byte a=255)
        {
            Hue = h;
            Saturation = s;
            Luminosity = l;

            Alpha = a;
        }

        public ColorHSL(Color color)
        {
            float _R = (color.R / 255f);
            float _G = (color.G / 255f);
            float _B = (color.B / 255f);
            Alpha = color.A;

            float _Min = Math.Min(Math.Min(_R, _G), _B);
            float _Max = Math.Max(Math.Max(_R, _G), _B);
            float _Delta = _Max - _Min;

            Hue = 0;
            Saturation = 0;
            Luminosity = ((_Max + _Min) / 2.0f);

            if (_Delta != 0)
            {
                if (Luminosity < 0.5f)
                {
                    Saturation = (float)(_Delta / (_Max + _Min));
                }
                else
                {
                    Saturation = (float)(_Delta / (2.0f - _Max - _Min));
                }


                if (_R == _Max)
                {
                    Hue = (_G - _B) / _Delta;
                }
                else if (_G == _Max)
                {
                    Hue = 2f + (_B - _R) / _Delta;
                }
                else if (_B == _Max)
                {
                    Hue = 4f + (_R - _G) / _Delta;
                }
            }
        }

        /// <summary>
        /// Clamp a value to 0-255
        /// </summary>
        int Clamp(int i)
        {
            if (i < 0) return 0;
            if (i > 255) return 255;
            return i;
        }

        public Color ToRgb()
        {
            byte r, g, b;
            if (Saturation == 0)
            {
                r = (byte)Math.Round(Luminosity * 255d);
                g = (byte)Math.Round(Luminosity * 255d);
                b = (byte)Math.Round(Luminosity * 255d);
            }
            else
            {
                double t1, t2;
                double th = Hue / 6.0d;

                if (Luminosity < 0.5d)
                {
                    t2 = Luminosity * (1d + Saturation);
                }
                else
                {
                    t2 = (Luminosity + Saturation) - (Luminosity * Saturation);
                }
                t1 = 2d * Luminosity - t2;

                double tr, tg, tb;
                tr = th + (1.0d / 3.0d);
                tg = th;
                tb = th - (1.0d / 3.0d);

                tr = ColorCalc(tr, t1, t2);
                tg = ColorCalc(tg, t1, t2);
                tb = ColorCalc(tb, t1, t2);
                r = (byte)Math.Round(tr * 255d);
                g = (byte)Math.Round(tg * 255d);
                b = (byte)Math.Round(tb * 255d);
            }
            return new Color(r, g, b, Alpha);
        }
        private static double ColorCalc(double c, double t1, double t2)
        {

            if (c < 0) c += 1d;
            if (c > 1) c -= 1d;
            if (6.0d * c < 1.0d) return t1 + (t2 - t1) * 6.0d * c;
            if (2.0d * c < 1.0d) return t2;
            if (3.0d * c < 2.0d) return t1 + (t2 - t1) * (2.0d / 3.0d - c) * 6.0d;
            return t1;
        }
    }
}
