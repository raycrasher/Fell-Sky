using Artemis;
using FellSky.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FellSky.Framework;

namespace FellSky.Services
{
    public class SpaceBackgroundGeneratorService
    {
        private readonly ContentManager Content;
        private readonly GraphicsDevice Device;
        private readonly Effect NebulaEffect;
        private readonly SpriteBatch SpriteBatch;
        private readonly Effect StarEffect;

        public SpaceBackgroundGeneratorService()
        {
            Device = ServiceLocator.Instance.GetService<GraphicsDevice>();
            Content = ServiceLocator.Instance.GetService<ContentManager>();
            SpriteBatch = ServiceLocator.Instance.GetService<SpriteBatch>();
            NebulaEffect = Content.Load<Effect>("Fx/SpaceGen_Nebula");
            StarEffect = Content.Load<Effect>("Fx/SpaceGen_Stars");

        }

        public Entity GenerateBackground(EntityWorld world, int seed)
        {
            return GenerateBackground(world, seed, new Point(Device.Viewport.Width, Device.Viewport.Height));
        }

        public Entity GenerateBackground(EntityWorld world, float starDensity, params NebulaParameters[] nebulae)
        {
            var entity = world.CreateEntity();
            entity.AddComponent(new Transform());

            var bgTex = GenerateBackgroundTexture(new Point(Device.Viewport.Width, Device.Viewport.Height), starDensity, nebulae);
            entity.AddComponent(new SpriteComponent { Texture = bgTex, TextureRect = new Rectangle(0, 0, bgTex.Width, bgTex.Height) });
            entity.AddComponent(new BackgroundComponent { Parallax = 0, FillViewPort = false });
            entity.Refresh();
            return entity;
        }

        public Entity GenerateBackground(EntityWorld world, int seed, Point size)
        {
            var entity = world.CreateEntity();
            entity.AddComponent(new Transform());

            var bgTex = GenerateBackgroundTexture(seed, size);
            entity.AddComponent(new SpriteComponent { Texture = bgTex, TextureRect = new Rectangle(0, 0, bgTex.Width, bgTex.Height) });
            entity.AddComponent(new BackgroundComponent { Parallax = 0, FillViewPort = false });
            entity.Refresh();
            return entity;
        }

        public Texture2D GenerateBackgroundTexture(int seed, Point size)
        {
            var rng = new Random(seed);

            var nebulae = Enumerable.Range(1, 4).Select(i => new NebulaParameters(rng)).ToArray();

            return GenerateBackgroundTexture(size, rng.NextFloat(0.001f, 0.01f), nebulae);
        }

        public Texture2D GenerateBackgroundTexture(Point size, float starDensity, params NebulaParameters[] nebulae)
        {
            var rendertarget = new RenderTarget2D(Device, size.X, size.Y);
            Device.SetRenderTarget(rendertarget);
            Device.Clear(Color.Black);

            StarEffect.Parameters["uRenderScale"].SetValue((float)Math.Max(size.X, size.Y));
            StarEffect.Parameters["uDensity"].SetValue(starDensity);
            NebulaEffect.Parameters["uRenderScale"].SetValue((float)Math.Max(size.X, size.Y));

            SpriteBatch.Begin(effect: StarEffect, blendState: BlendState.Additive);
            SpriteBatch.FillRectangle(new Rectangle(Point.Zero, size), Color.Black);
            SpriteBatch.End();

            foreach (var nebula in nebulae)
            {
                NebulaEffect.Parameters["uColor"].SetValue(nebula.Color.ToVector3());
                NebulaEffect.Parameters["uOffset"].SetValue(nebula.Offset);
                NebulaEffect.Parameters["uFalloff"].SetValue(nebula.Falloff);
                NebulaEffect.Parameters["uIntensity"].SetValue(nebula.Intensity);
                NebulaEffect.Parameters["uScale"].SetValue(nebula.Scale);

                SpriteBatch.Begin(effect: NebulaEffect, blendState: BlendState.Additive);
                SpriteBatch.FillRectangle(new Rectangle(Point.Zero, size), Color.Black);
                SpriteBatch.End();
            }
            Device.SetRenderTarget(null);
            return rendertarget;
        }
    }

    public class NebulaParameters
    {
        public Color Color;
        public Vector2 Offset;
        public float Falloff;
        public float Intensity;
        public float Scale;

        public NebulaParameters() { }
        public NebulaParameters(Color color, Vector2 offset, float falloff, float intensity, float scale)
        {
            Color = color;
            Offset = offset;
            Falloff = falloff;
            Intensity = intensity;
            Scale = scale;
        }

        public NebulaParameters(Random rng)
        {
            Color = rng.NextRandomColor();
            Offset = new Vector2(rng.Next(-1000, 1000), rng.Next(-1000, 1000));
            Falloff = rng.NextFloat(3, 6);
            Intensity = (float)(rng.NextDouble() * 0.2 + 1.0);
            Scale = rng.NextFloat(0.01f, 4f);
        }
    }
}
