using Artemis;
using Microsoft.Xna.Framework.Graphics;
using FellSky.Components;

namespace FellSky.Systems
{
    [Artemis.Attributes.ArtemisEntitySystem(ExecutionType = Artemis.Manager.ExecutionType.Synchronous, GameLoopType = Artemis.Manager.GameLoopType.Draw, Layer = 10)]
    public class GenericDrawableRendererSystem : Artemis.System.EntityComponentProcessingSystem<GenericDrawableComponent>
    {
        private SpriteBatch _spritebatch;
        private GraphicsDevice _device;
        private CameraComponent _camera;

        public string CameraTag { get; set; }

        public GenericDrawableRendererSystem(SpriteBatch spriteBatch, GraphicsDevice device, string cameraTag)
        {
            _spritebatch = spriteBatch;
            _device = device;
            CameraTag = cameraTag;
        }

        protected override void Begin()
        {
            base.Begin();
            _camera = EntityWorld.GetCamera(CameraTag);
        }
        public override void Process(Entity entity, GenericDrawableComponent drawable)
        {
            if (_camera != null) _spritebatch.Begin(transformMatrix: _camera.GetViewMatrix(1.0f));
            else _spritebatch.Begin();
            drawable.DrawFunction(_device, _spritebatch, entity);
            _spritebatch.End();
        }
    }
}
