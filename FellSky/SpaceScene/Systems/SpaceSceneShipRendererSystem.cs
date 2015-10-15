using Artemis;
using FellSky.Common;
using FellSky.EntityComponents;
using FellSky.EntitySystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FellSky.SpaceScene.Systems
{
    [Artemis.Attributes.ArtemisEntitySystem(
        ExecutionType = Artemis.Manager.ExecutionType.Synchronous, 
        GameLoopType = Artemis.Manager.GameLoopType.Draw,
        Layer = 10
        )]
    public class SpaceSceneShipRendererSystem : Artemis.System.EntitySystem
    {
        SpriteBatch _spriteBatch;
        SpriteBatch _spriteOverlayBatch;
        RenderTarget2D _spriteOverlayRenderTarget;
        GraphicsDevice _device;
        Camera2D _camera;

        public SpaceSceneShipRendererSystem()
            : base(Aspect.All(typeof(ShipSpriteComponent)))
        {
        }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            _device.SetRenderTarget(null);
            DrawSprites(entities, _spriteBatch);
            
            //_device.SetRenderTarget(_spriteOverlayRenderTarget);
            //_device.Clear(Color.Black);
            //DrawOverlays(entities, _spriteBatch);
            //
            //_device.SetRenderTarget(null);
            
        }
        /*
        private void DrawOverlays(IDictionary<int, Entity> entities)
        {
            
            for (int i = 0; i < entities.Count; i++)
            {
                _device.SetRenderTarget(_spriteOverlayRenderTarget);
                _device.Clear(Color.Black);

                _spriteOverlayBatch.Begin();
                var entity = entities[i];
                var component = entity.GetComponent<ShipSpriteComponent>();
                var transform = entity.GetComponent<Transform>();

                for(int j=0; j < component.Overlays.Count; j++)
                {
                    var sprite = component.Overlays[j];
                    sprite.Sprite.Draw(_spriteOverlayBatch, sprite.Transform.Position + transform.Position, sprite.Transform.Rotation + transform.Rotation, sprite.Transform.Scale * transform.Scale, sprite.Transform.Origin + transform.Origin, sprite.Color);
                }

                _spriteOverlayBatch.End();

            }
            _device.SetRenderTarget(null);
        }*/

        private void DrawSprites(IDictionary<int, Entity> entities, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            for (int idxEntity = 0; idxEntity < entities.Count; idxEntity++)
            {
                var entity = entities[idxEntity];
                var spriteComponent = entity.GetComponent<ShipSpriteComponent>();
                var transform = entity.GetComponent<Transform>();
                var shipMatrix = transform.Matrix;

                for (int idxSubSprite = 0; idxSubSprite < spriteComponent.Sprite.SubSprites.Count; idxSubSprite++)
                {
                    var sprite = spriteComponent.Sprite.SubSprites[idxSubSprite];
                    sprite.Sprite.Draw(spriteBatch, sprite.Transform.Position + transform.Position, sprite.Transform.Rotation + transform.Rotation, sprite.Transform.Scale * transform.Scale, sprite.Transform.Origin + transform.Origin, sprite.Color);
                }
            }
            spriteBatch.End();
        }

        public override void LoadContent()
        {
            _camera = BlackBoard.GetEntry<Camera2D>("Camera2D");
            _device = Game.Instance.GraphicsDevice;
            _spriteBatch = new SpriteBatch(_device);
            _spriteOverlayRenderTarget = new RenderTarget2D(_device,
                _device.PresentationParameters.BackBufferWidth,
                _device.PresentationParameters.BackBufferHeight,
                false,
                _device.PresentationParameters.BackBufferFormat,
                _device.PresentationParameters.DepthStencilFormat
                );
            _spriteOverlayBatch = new SpriteBatch(_device);
            base.LoadContent();
        }

        public override void UnloadContent()
        {
            _spriteOverlayBatch.Dispose();
            _spriteBatch.Dispose();
            _spriteOverlayRenderTarget.Dispose();
            base.UnloadContent();
        }
    }
}
