using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Services
{
    public interface INoise
    {
        double Noise(double x, double y, double z);
    }

    public class SpaceBackgroundGeneratorService
    {
        public Color[] Colors { get; set; }
        INoise noiseGenerator;

        Random Rng;
        private GraphicsDevice _device;

        public float TaskProgress { get; private set; }

        public SpaceBackgroundGeneratorService(GraphicsDevice device, Color[] colors, double scale, double intensity, double falloff, int seed)
        {
            _device = device;
            Colors = colors;
            Scale = scale;
            Intensity = intensity;
            Falloff = falloff;
            Seed = seed;
            Rng = new MersenneTwisterRng(Seed);
            noiseGenerator = new Perlin(Rng);
            //noiseGenerator = new SimplexNoiseGenerator();
        }

        Color Field(double r, double g, double b, double x, double y, double intensity, double falloff)
        {
            var i = Math.Min(1, RecursiveField(x, y, 5, 2) * intensity);
            i = Math.Pow(i, falloff);
            var c = new Color(
                (int)r,
                (int)g,
                (int)b,
                (int)MathHelper.Clamp((float)i * 255,0, 255));
            return c;
        }

        private double RecursiveField(double x, double y, double depth, double divisor)
        {
            if (depth <= 0)
                return noiseGenerator.Noise(x / divisor, y / divisor, 0);
            var displace = RecursiveField(x, y, depth - 1, divisor / 2);
            return noiseGenerator.Noise(x / divisor + displace, y / divisor + displace, 0);
        }

        public Texture2D GenerateNebula(int Width, int Height)
        {

            var bitmap = Enumerable.Repeat(Color.Black, Width * Height).ToArray();
            GenerateStars(bitmap, Width, Height);

            foreach (var color in Colors)
            {
                Parallel.For(0, Height, y =>
                {
                    Parallel.For(0, Width, x =>
                    {
                        bitmap[y * Width + x] = ApplyAlphaColor(
                            bitmap[y * Width + x],
                            Field(color.R, color.G, color.B, x / Scale, y / Scale, Intensity, Falloff));
                    });
                });
            }
            var tex2d = new Texture2D(_device, Width, Height, false, SurfaceFormat.Color);
            tex2d.SetData(bitmap);
            return tex2d;
        }

        void GenerateStars(Color[] bitmap, int Width, int Height)
        {
            var numStars = (int)(Scale);
            for (int i = 0; i < numStars / 5; i++)
            {
                var x = Rng.Next(0, Width - 2);
                var y = Rng.Next(0, Height - 2);

                bitmap[y * Width + x] = ApplyAlphaColor(bitmap[y * Width + x], new Color(Rng.Next(100, 255), Rng.Next(100, 255), Rng.Next(100, 255), Rng.Next(100, 255)));
                x++;
                bitmap[y * Width + x] = ApplyAlphaColor(bitmap[y * Width + x], new Color(Rng.Next(100, 255), Rng.Next(100, 255), Rng.Next(100, 255), Rng.Next(100, 255)));
                x--; y++;
                bitmap[y * Width + x] = ApplyAlphaColor(bitmap[y * Width + x], new Color(Rng.Next(100, 255), Rng.Next(100, 255), Rng.Next(100, 255), Rng.Next(100, 255)));
                x++;
                bitmap[y * Width + x] = ApplyAlphaColor(bitmap[y * Width + x], new Color(Rng.Next(0, 255), Rng.Next(0, 255), Rng.Next(0, 255), Rng.Next(0, 255)));
            }

            for (int i = 0; i < numStars / 10; i++)
            {
                var x = Rng.Next(0, Width - 2);
                var y = Rng.Next(0, Height - 2);

                bitmap[y * Width + x] = ApplyAlphaColor(bitmap[y * Width + x], new Color(Rng.Next(100, 255), Rng.Next(100, 255), Rng.Next(100, 255), Rng.Next(100, 255)));
                x++;
                bitmap[y * Width + x] = ApplyAlphaColor(bitmap[y * Width + x], new Color(Rng.Next(100, 255), Rng.Next(100, 255), Rng.Next(100, 255), Rng.Next(100, 255)));
                x--; y++;
                bitmap[y * Width + x] = ApplyAlphaColor(bitmap[y * Width + x], new Color(Rng.Next(100, 255), Rng.Next(100, 255), Rng.Next(100, 255), Rng.Next(100, 255)));
                x++;
                bitmap[y * Width + x] = ApplyAlphaColor(bitmap[y * Width + x], new Color(Rng.Next(100, 255), Rng.Next(100, 255), Rng.Next(100, 255), Rng.Next(0, 255)));
            }

            for (int i = 0; i < numStars; i++)
            {
                var x = Rng.Next(0, Width - 2);
                var y = Rng.Next(0, Height - 2);
                bitmap[y * Width + x] = ApplyAlphaColor(
                    bitmap[y * Width + x],
                    new Color(
                        Rng.Next(200, 255),
                        Rng.Next(200, 255),
                        Rng.Next(200, 255),
                        Rng.Next(0, 255)
                        ));
            }
        }

        

        class Perlin: INoise
        {
            double[] p = new double[256];
            double[] perm = new double[513];

            Random rng;

            public Perlin(Random r)
            {
                rng = r;
                for (var i = 0; i < 256; i++)
                {
                    p[i] = (double)rng.NextDouble() * 256;
                }

                for (var i = 0; i < 512; i++)
                {
                    perm[i] = p[i & 255];
                }
            }

            double[][] grad3 = new double[][]
            {
                new double[]{1, 1, 0},
                new double[]{-1, 1, 0},
                new double[]{1, -1, 0},
                new double[]{-1, -1, 0},
                new double[]{1, 0, 1},
                new double[]{-1, 0, 1},
                new double[]{1, 0, -1},
                new double[]{-1, 0, -1},
                new double[]{0, 1, 1},
                new double[]{0, -1, 1},
                new double[]{0, 1, -1},
                new double[]{0, -1, -1}
            };

            double Dot(double[] g, double x, double y, double z)
            {
                return g[0] * x + g[1] * y + g[2] * z;
            }

            double Mix(double a, double b, double t)
            {
                return (1.0f - t) * a + t * b;
            }

            double Fade(double t)
            {
                return t * t * t * (t * (t * 6.0f - 15.0f) + 10.0f);
            }

            double NoiseImp(double x, double y, double z)
            {
                int X = (int)Math.Floor(x);
                int Y = (int)Math.Floor(y);
                int Z = (int)Math.Floor(z);
                x = x - X;
                y = y - Y;
                z = z - Z;
                X = (int)X & 255;
                Y = (int)Y & 255;
                Z = (int)Z & 255;
                var gi000 = this.perm[(int)(X + this.perm[(int)(Y + this.perm[(int)(Z)])])] % 12;
                var gi001 = this.perm[(int)(X + this.perm[(int)(Y + this.perm[(int)(Z + 1)])])] % 12;
                var gi010 = this.perm[(int)(X + this.perm[(int)(Y + 1 + this.perm[(int)(Z)])])] % 12;
                var gi011 = this.perm[(int)(X + this.perm[(int)(Y + 1 + this.perm[(int)(Z + 1)])])] % 12;
                var gi100 = this.perm[(int)(X + 1 + this.perm[(int)(Y + this.perm[(int)(Z)])])] % 12;
                var gi101 = this.perm[(int)(X + 1 + this.perm[(int)(Y + this.perm[(int)(Z + 1)])])] % 12;
                var gi110 = this.perm[(int)(X + 1 + this.perm[(int)(Y + 1 + this.perm[(int)(Z)])])] % 12;
                var gi111 = this.perm[(int)(X + 1 + this.perm[(int)(Y + 1 + this.perm[(int)(Z + 1)])])] % 12;
                var n000 = this.Dot(this.grad3[(int)(gi000)], x, y, z);
                var n100 = this.Dot(this.grad3[(int)(gi100)], x - 1, y, z);
                var n010 = this.Dot(this.grad3[(int)(gi010)], x, y - 1, z);
                var n110 = this.Dot(this.grad3[(int)(gi110)], x - 1, y - 1, z);
                var n001 = this.Dot(this.grad3[(int)(gi001)], x, y, z - 1);
                var n101 = this.Dot(this.grad3[(int)(gi101)], x - 1, y, z - 1);
                var n011 = this.Dot(this.grad3[(int)(gi011)], x, y - 1, z - 1);
                var n111 = this.Dot(this.grad3[(int)(gi111)], x - 1, y - 1, z - 1);
                var u = this.Fade(x);
                var v = this.Fade(y);
                var w = this.Fade(z);
                var nx00 = this.Mix(n000, n100, u);
                var nx01 = this.Mix(n001, n101, u);
                var nx10 = this.Mix(n010, n110, u);
                var nx11 = this.Mix(n011, n111, u);
                var nxy0 = this.Mix(nx00, nx10, v);
                var nxy1 = this.Mix(nx01, nx11, v);
                var nxyz = this.Mix(nxy0, nxy1, w);
                return nxyz;
            }

            public double Noise(double x, double y, double z)
            {
                return 0.5f * NoiseImp(x, y, z) + 0.5f;
            }
        }

        public double Scale { get; set; }

        public double Intensity { get; set; }

        public double Falloff { get; set; }

        public int Seed { get; set; }

        public static Color ApplyAlphaColor(Color bottom, Color top)
        {
            var bottomR = (bottom.R / 255f);
            var bottomG = (bottom.G / 255f);
            var bottomB = (bottom.B / 255f);
            var bottomA = (bottom.A / 255f);
            var topR = (top.R / 255f);
            var topG = (top.G / 255f);
            var topB = (top.B / 255f);
            var topA = (top.A / 255f);


            var newR = (topR * topA) + (bottomR * bottomA) * (1 - topA);
            var newG = (topG * topA) + (bottomG * bottomA) * (1 - topA);
            var newB = (topB * topA) + (bottomB * bottomA) * (1 - topA);
            var newA = topA + bottomA * (1 - topA);


            return new Color(newR, newG, newB, newA);
        }
    }
}
