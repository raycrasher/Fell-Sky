using FellSky.Mechanics.Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using FellSky.EntityComponents;
using Microsoft.Xna.Framework.Graphics;
using FellSky.EntitySystems;
using Microsoft.Xna.Framework;

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
        List<ShipSpriteComponent.SpriteRecord> _renderList = new List<ShipSpriteComponent.SpriteRecord>();
        private Camera2D _camera;

        public SpaceSceneShipRendererSystem()
            : base(Aspect.All(typeof(ShipSpriteComponent)))
        {
        }

        protected override void Begin()
        {
            _spriteBatch.Begin();
            base.Begin();
        }

        protected override void End()
        {
            base.End();
            _spriteBatch.End();
        }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            _renderList.Clear();
            var screenBounds = _camera.ScreenBounds;

            CullEntities(_renderList, entities, screenBounds);

            _device.SetRenderTarget(null);
            DrawSprites(_renderList, _spriteBatch);
            
            _device.SetRenderTarget(_spriteOverlayRenderTarget);
            DrawOverlays(_renderList, _spriteBatch);
            
            _device.SetRenderTarget(null);
            _renderList.Clear();
        }

        private void DrawOverlays(List<ShipSpriteComponent.SpriteRecord> renderList, SpriteBatch spriteBatch)
        {
            _spriteOverlayBatch.Begin();
            for (int i = 0; i < renderList.Count; i++)
            {
                var sprite = renderList[i];
                sprite.Sprite.Draw(_spriteOverlayBatch, sprite.Transform.Position, sprite.Transform.Rotation, sprite.Transform.Scale);   
            }
            _spriteOverlayBatch.End();
        }

        private void DrawSprites(List<ShipSpriteComponent.SpriteRecord> renderList, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            for (int i = 0; i < renderList.Count; i++)
            {
                var sprite = renderList[i];
                sprite.Sprite.Draw(_spriteOverlayBatch, sprite.Transform.Position, sprite.Transform.Rotation, sprite.Transform.Scale);
            }
            _spriteOverlayBatch.End();
            spriteBatch.End();
        }

        private void CullEntities(List<ShipSpriteComponent.SpriteRecord> renderList, IDictionary<int, Entity> entities, Rectangle screenBounds)
        {
            for(int i = 0; i < entities.Count; i++)
            {
                var component = entities[i].GetComponent<ShipSpriteComponent>();
                for(int j = 0; j < component.SpriteEntries.Count; j++)
                {
                    var sprite = component.SpriteEntries[j];
                    if (IsWithinScreenBounds(sprite, screenBounds))
                        renderList.Add(sprite);
                }
                
            }
        }

        private bool IsWithinScreenBounds(ShipSpriteComponent.SpriteRecord sprite, Rectangle screenBounds)
        {
            return screenBounds.Intersects(_camera.Transform.GetMatrix().)
        }

        public override void LoadContent()
        {
            _camera = BlackBoard.GetEntry<Camera2D>("Camera2D");
            _renderList.Capacity = 1024;
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
