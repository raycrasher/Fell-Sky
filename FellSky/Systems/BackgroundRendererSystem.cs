using Artemis;
using FellSky.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Systems
{
    public class BackgroundRendererSystem: Artemis.System.EntitySystem
    {
        private SpriteBatch _batch;

        public BackgroundRendererSystem(SpriteBatch batch, string cameraTag)
            : base(Aspect.All(typeof(BackgroundComponent), typeof(SpriteComponent), typeof(Transform)))
        {
            _batch = batch;
            CameraTag = cameraTag;
        }

        public string CameraTag { get; set; }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            var camera = EntityWorld.GetCamera(CameraTag);

            _batch.Begin(sortMode: SpriteSortMode.BackToFront);

            foreach(var entity in entities.Values)
            {                
                var bg = entity.GetComponent<BackgroundComponent>();
                var xform = entity.GetComponent<Transform>();
                var sprite = entity.GetComponent<SpriteComponent>();
                //var cameraMatrix = camera.GetViewMatrix(bg.Parallax);

                Matrix adjust = Matrix.Identity;
                if(bg.FillViewPort)
                {
                    float scale = 1;
                    if (sprite.TextureRect.Width < camera.ScreenSize.X)
                        scale = camera.ScreenSize.X / sprite.TextureRect.Width;
                    if (sprite.TextureRect.Height < camera.ScreenSize.Y)
                        scale = MathHelper.Max(camera.ScreenSize.X / sprite.TextureRect.Height, scale);
                    adjust = Matrix.CreateScale(scale);
                }

                sprite.Draw(_batch, xform.Matrix * adjust, bg.Color, bg.SpriteEffect, bg.Depth);
            }
            _batch.End();
        }
    }
}
