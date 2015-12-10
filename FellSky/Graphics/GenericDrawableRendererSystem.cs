using System;
using Artemis;
using Microsoft.Xna.Framework.Graphics;
using FellSky.Graphics;

namespace FellSky.Graphics
{
    [Artemis.Attributes.ArtemisEntitySystem(ExecutionType = Artemis.Manager.ExecutionType.Synchronous, GameLoopType = Artemis.Manager.GameLoopType.Draw, Layer = 10)]
    public class GenericDrawableRendererSystem : Artemis.System.EntityComponentProcessingSystem<GenericDrawableComponent>
    {
        private SpriteBatch _spritebatch;
        private GraphicsDevice _device;
        private Camera2D _camera;

        public override void LoadContent()
        {
            base.LoadContent();
            _spritebatch = BlackBoard.GetService<SpriteBatch>();
            _device = BlackBoard.GetService<GraphicsDevice>();
        }
        protected override void Begin()
        {
            base.Begin();
            _camera = BlackBoard.GetEntry<Camera2D>(Camera2D.PlayerCameraName);
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
