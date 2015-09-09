using FellSky.Mechanics.Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using FellSky.EntityComponents;
using Microsoft.Xna.Framework.Graphics;

namespace FellSky.SpaceScene.Systems
{
    public class SpaceSceneShipRendererSystem : Artemis.System.EntityComponentProcessingSystem<ShipSpriteComponent>
    {
        SpriteBatch _spriteBatch;

        public SpaceSceneShipRendererSystem(GraphicsDevice device=null)
        {
            _spriteBatch = new SpriteBatch(device ?? Game.Instance.GraphicsDevice);
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

        public override void Process(Entity entity, ShipSpriteComponent ship)
        {
            foreach(var spriteEntry in ship.SpriteEntries)
            {
                spriteEntry.Sprite.Draw(_spriteBatch, spriteEntry.Transform.Position, spriteEntry.Transform.Rotation, spriteEntry.Transform.Scale);
            }
        }
    }
}
