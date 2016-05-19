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

        private Texture2D GenerateBackgroundTexture(int seed, Point size)
        {
            var Rng = new Random();
            var rendertarget = new RenderTarget2D(Device, size.X, size.Y);
            Device.SetRenderTarget(rendertarget);
            Device.Clear(Color.Black);

            var colors = Enumerable.Range(0, Rng.Next(1, 3)).Select(i => Rng.NextRandomColor());

            StarEffect.Parameters["uRenderScale"].SetValue((float)Math.Max(size.X, size.Y));
            StarEffect.Parameters["uDensity"].SetValue(Rng.NextFloat(0.001f,0.01f));
            NebulaEffect.Parameters["uRenderScale"].SetValue((float)Math.Max(size.X, size.Y));

            SpriteBatch.Begin(effect: StarEffect, blendState: BlendState.Additive);
            SpriteBatch.FillRectangle(new Rectangle(Point.Zero, size), Color.Black);
            SpriteBatch.End();

            foreach (var c in colors)
            {                
                NebulaEffect.Parameters["uColor"].SetValue(c.ToVector3());
                NebulaEffect.Parameters["uOffset"].SetValue(new Vector2(Rng.Next(-1000, 1000), Rng.Next(-1000, 1000)));
                NebulaEffect.Parameters["uFalloff"].SetValue(Rng.NextFloat(3,6));
                NebulaEffect.Parameters["uIntensity"].SetValue((float)(Rng.NextDouble() * 0.2 + 1.0));
                NebulaEffect.Parameters["uScale"].SetValue(Rng.NextFloat(0.01f,4f));

                SpriteBatch.Begin(effect: NebulaEffect, blendState: BlendState.Additive);
                SpriteBatch.FillRectangle(new Rectangle(Point.Zero, size), Color.Black);
                SpriteBatch.End();
            }

            Device.SetRenderTarget(null);
            return rendertarget;
        }
    }
}
