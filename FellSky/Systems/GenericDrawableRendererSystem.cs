using Artemis;
using Microsoft.Xna.Framework.Graphics;
using FellSky.Components;
using FellSky.Services;
using FellSky.Framework;
using Microsoft.Xna.Framework;

namespace FellSky.Systems
{
    public class GenericDrawableRendererSystem : Artemis.System.EntityComponentProcessingSystem<GenericDrawableComponent>
    {
        private SpriteBatch _batch;
        private GraphicsDevice _device;

        public GenericDrawableRendererSystem()
        {
            _batch = ServiceLocator.Instance.GetService<SpriteBatch>();
            _device = ServiceLocator.Instance.GetService<GraphicsDevice>();
        }

        public override void Process(Entity entity, GenericDrawableComponent drawable)
        {
            var camera = EntityWorld.TagManager.GetEntity(Constants.ActiveCameraTag).GetComponent<Camera>();
            _batch.Begin(effect: camera.SpriteBatchBasicEffect, rasterizerState: RasterizerState.CullNone);
            drawable.DrawFunction(_device, _batch, entity);
            _batch.End();
        }
    }
}
