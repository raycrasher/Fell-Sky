using Artemis;
using FellSky.Common;
using FellSky.EntityComponents;
using FellSky.EntitySystems;
using FellSky.Graphics;
using FellSky.Mechanics.Ships;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System;

namespace FellSky.Systems
{
    [Artemis.Attributes.ArtemisEntitySystem(
        ExecutionType = Artemis.Manager.ExecutionType.Synchronous, 
        GameLoopType = Artemis.Manager.GameLoopType.Draw,
        Layer = 10
        )]
    public class ShipRendererSystem : Artemis.System.EntitySystem
    {
        SpriteBatch _spriteBatch;
        GraphicsDevice _device;
        Camera2D _camera;
        private Matrix _matrix;

        public ShipRendererSystem()
            : base(Aspect.All(typeof(Ship), typeof(ShipSpriteComponent)))
        {
        }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            if (_camera == null) return;
            _device.SetRenderTarget(null);
            _matrix = _camera.GetViewMatrix(1.0f);
            DrawThrusters(entities, _spriteBatch);
            DrawHulls(entities, _spriteBatch);
            
        }

        private void DrawThrusters(IDictionary<int, Entity> entities, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(blendState: BlendState.Additive, transformMatrix: _matrix);

            for (int idxEntity = 0; idxEntity < entities.Count; idxEntity++)
            {
                var ship = entities[idxEntity];
                var shipSprite = ship.GetComponent<ShipSpriteComponent>();
                var xform = ship.GetComponent<Transform>();
                var shipMatrix = xform.Matrix;

                foreach(var thruster in shipSprite.Ship.Thrusters.Where(s=>s.ThrustPercent>0))
                {
                    var color = Color.Lerp(new Color(thruster.Color, 0), thruster.Color, MathHelper.Clamp(thruster.ThrustPercent, 0, 1));
                    thruster.Sprite.Draw(spriteBatch, thruster.Transform.Matrix * shipMatrix, color);
                }
            }
            spriteBatch.End();
        }
        

        private void DrawHulls(IDictionary<int, Entity> entities, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(blendState: BlendState.AlphaBlend, transformMatrix: _matrix);
            for (int idxEntity = 0; idxEntity < entities.Count; idxEntity++)
            {
                var ship = entities[idxEntity];
                var shipSprite = ship.GetComponent<ShipSpriteComponent>();
                var xform = ship.GetComponent<Transform>();
                var shipMatrix = xform.Matrix;

                foreach (var hull in shipSprite.Ship.Hulls)
                {
                    hull.Sprite.Draw(spriteBatch, hull.Transform.Matrix * shipMatrix, hull.Color);
                }
            }
            spriteBatch.End();
        }

        /// <summary>
        /// Loads content
        /// </summary>
        public override void LoadContent()
        {
            _camera = BlackBoard.GetEntry<Camera2D>(Camera2D.PlayerCameraName);
            _device = Game.Instance.GraphicsDevice;
            _spriteBatch = new SpriteBatch(_device);
            base.LoadContent();
        }

        public override void UnloadContent()
        {
            _spriteBatch.Dispose();
            base.UnloadContent();
        }
    }
}
