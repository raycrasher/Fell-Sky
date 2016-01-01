using Artemis;
using FellSky.Graphics;
using FellSky.Ships;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System;

namespace FellSky.Ships
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

        protected override void Begin()
        {
            base.Begin();
            _camera = BlackBoard.GetEntry<Camera2D>(Camera2D.PlayerCameraName);
        }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            if (_camera == null) return;
            //_device.SetRenderTarget(null);
            _matrix = _camera.GetViewMatrix(1.0f);
            _spriteBatch.Begin(SpriteSortMode.BackToFront, blendState: BlendState.AlphaBlend, transformMatrix: _matrix);
            DrawThrusters(entities.Values);
            DrawHulls(entities.Values);
            _spriteBatch.End();
        }

        private void DrawThrusters(ICollection<Entity> entities)
        {
            foreach (var ship in entities)
            {
                var shipSprite = ship.GetComponent<ShipSpriteComponent>();
                var xform = ship.GetComponent<Transform>();
                var shipMatrix = xform.Matrix;

                foreach(var thruster in shipSprite.Ship.Thrusters.Where(s=>s.ThrustPercent>0))
                {
                    var color = Color.Lerp(new Color(thruster.Color, 0), thruster.Color, MathHelper.Clamp(thruster.ThrustPercent, 0, 1));
                    thruster.Sprite.Draw(batch: _spriteBatch, matrix: thruster.Transform.Matrix * shipMatrix, color: thruster.Color, depth: thruster.Depth);
                }
            }
        }
        
        

        private void DrawHulls(ICollection<Entity> entities)
        {
            
            foreach (var ship in entities)
            {
                var shipSprite = ship.GetComponent<ShipSpriteComponent>();
                var xform = ship.GetComponent<Transform>();
                var shipMatrix = xform.Matrix;
                var shipComponent = ship.GetComponent<Ship>();
                var basedecal = shipComponent.BaseDecalColor.ToVector4();
                var trimdecal = shipComponent.TrimDecalColor.ToVector4();

                foreach (var hull in shipSprite.Ship.Hulls)
                {
                    var color = hull.Color;
                    switch (hull.ColorType)
                    {
                        case Parts.HullColorType.BaseDecal:
                            color = new Color(basedecal * color.ToVector4());
                            break;
                        case Parts.HullColorType.TrimDecal:
                            color = new Color(trimdecal * color.ToVector4());
                            break;
                    }
                    hull.Sprite.Draw(batch: _spriteBatch, matrix: hull.Transform.Matrix * shipMatrix, color:color, depth: hull.Depth, effects: hull.SpriteEffect);
                }
            }
        }

        /// <summary>
        /// Loads content
        /// </summary>
        public override void LoadContent()
        {
            _device = BlackBoard.GetService<GraphicsDevice>();
            _spriteBatch = BlackBoard.GetService<SpriteBatch>();
            base.LoadContent();
        }

        public override void UnloadContent()
        {
            _spriteBatch.Dispose();
            base.UnloadContent();
        }
    }
}
