using FellSky.EntityComponents;
using System;
using Artemis;
using Microsoft.Xna.Framework.Graphics;
using FellSky.Graphics;

namespace FellSky.Systems
{
    [Artemis.Attributes.ArtemisEntitySystem(ExecutionType = Artemis.Manager.ExecutionType.Synchronous, GameLoopType = Artemis.Manager.GameLoopType.Draw)]
    public class GenericDrawableRendererSystem : Artemis.System.EntityComponentProcessingSystem<GenericDrawableComponent>
    {
        private SpriteBatch _spritebatch;
        private GraphicsDevice _device;
        private Camera2D _camera;

        public override void LoadContent()
        {
            base.LoadContent();
            var serviceProvider = BlackBoard.GetEntry<IServiceProvider>("ServiceProvider");
            _spritebatch = serviceProvider.GetService<SpriteBatch>();
            _device = serviceProvider.GetService<GraphicsDevice>();
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
