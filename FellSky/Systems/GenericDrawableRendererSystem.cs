using Artemis;
using Microsoft.Xna.Framework.Graphics;
using FellSky.Components;
using FellSky.Services;

namespace FellSky.Systems
{
    public class GenericDrawableRendererSystem : Artemis.System.EntityComponentProcessingSystem<GenericDrawableComponent>
    {
        private SpriteBatch _spritebatch;
        private GraphicsDevice _device;

        public GenericDrawableRendererSystem()
        {
            _spritebatch = ServiceLocator.Instance.GetService<SpriteBatch>();
            _device = ServiceLocator.Instance.GetService<GraphicsDevice>();
        }

        public override void Process(Entity entity, GenericDrawableComponent drawable)
        {
            var camera = EntityWorld.TagManager.GetEntity(Constants.ActiveCameraTag).GetComponent<Camera>();
            if (camera != null) _spritebatch.Begin(transformMatrix: camera.GetViewMatrix(1.0f));
            else _spritebatch.Begin();
            drawable.DrawFunction(_device, _spritebatch, entity);
            _spritebatch.End();
        }
    }
}
