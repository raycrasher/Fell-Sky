using Artemis;
using FellSky.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System;
using FellSky.Game.Ships.Parts;
using FellSky.Components;
using FellSky.Services;

namespace FellSky.Systems
{
    public class ShipRendererSystem : Artemis.System.EntitySystem
    {
        SpriteBatch _spriteBatch;
        private Matrix _matrix;
        static ITimerService _timer;

        public ShipRendererSystem()
            : base(Aspect.All(typeof(ShipComponent)))
        {
            _spriteBatch = ServiceLocator.Instance.GetService<SpriteBatch>();
            _timer = ServiceLocator.Instance.GetService<ITimerService>();
        }


        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            var camera = EntityWorld.TagManager.GetEntity(Constants.ActiveCameraTag).GetComponent<Camera>();
            if (camera == null) return;
            _matrix = camera.GetViewMatrix(1.0f);
            _spriteBatch.Begin(SpriteSortMode.Deferred, blendState: BlendState.AlphaBlend, transformMatrix: _matrix, samplerState: SamplerState.AnisotropicClamp);

            foreach (var entity in entities.Values)
            {
                DrawShip(_spriteBatch, entity);
            }

            _spriteBatch.End();
        }

        public static void DrawShip(SpriteBatch spriteBatch, Entity ship)
        {
            var shipComponent = ship.GetComponent<ShipComponent>();
            var xform = ship.GetComponent<Transform>();
            var shipMatrix = xform.Matrix;

            foreach(var item in shipComponent.PartEntities)
            {
                if (item.Entity.HasComponent<HullComponent>())
                    DrawHull(spriteBatch, ship, item.Entity, ref shipMatrix);
                else if (item.Entity.HasComponent<ThrusterComponent>())
                    DrawThruster(spriteBatch, ship, item.Entity, ref shipMatrix);
            }
        }

        private static void DrawThruster(SpriteBatch spriteBatch, Entity ship, Entity thrusterEntity, ref Matrix shipMatrix)
        {            
            var shipComponent = ship.GetComponent<ShipComponent>();          
            var thrusterComponent = thrusterEntity.GetComponent<ThrusterComponent>();
            if (thrusterComponent.ThrustPercentage > 0 || thrusterComponent.Part.IsIdleModeOnZeroThrust)
            {
                var sprite = thrusterEntity.GetComponent<SpriteComponent>();
                var thruster = thrusterComponent.Part;

                _tmpXform.CopyValuesFrom(thruster.Transform);
                SpriteEffects fx = SpriteEffects.None;
                if (_tmpXform.Scale.X < 0)
                {
                    fx |= SpriteEffects.FlipHorizontally;
                    _tmpXform.Scale *= new Vector2(-1, 1);

                }
                if (_tmpXform.Scale.Y < 0)
                {
                    fx |= SpriteEffects.FlipVertically;
                    _tmpXform.Scale *= new Vector2(1, -1);
                }

                float colorAlpha = 0; 
                
                if (thrusterComponent.Part.IsIdleModeOnZeroThrust)
                {
                    _tmpXform.Scale *= new Vector2(MathHelper.Clamp(thrusterComponent.ThrustPercentage, 0.3f, 1), 1);
                    colorAlpha = MathHelper.Clamp(thrusterComponent.ThrustPercentage, 0.6f, 1);
                } else
                {
                    _tmpXform.Scale *= new Vector2(MathHelper.Clamp(thrusterComponent.ThrustPercentage, 0, 1), 1);
                    colorAlpha = thrusterComponent.ThrustPercentage;
                }

                if (!thrusterEntity.HasComponent<EditorComponent>())
                {
                    // do thruster graphic wobble
                    var time = thrusterComponent.GetHashCode() % 1000 + _timer.LastFrameUpdateTime.TotalGameTime.Milliseconds;
                    var amount = (float)Math.Sin((time % MathHelper.Pi * 2) * 0.5f);
                    _tmpXform.Scale *= 1 + amount * 0.1f;
                }
                sprite.Draw(batch: spriteBatch, matrix: _tmpXform.Matrix * shipMatrix, color: thruster.Color * colorAlpha, effects: fx);
                _tmpXform.Scale *= 0.8f;
                sprite.Draw(batch: spriteBatch, matrix: _tmpXform.Matrix * shipMatrix, color: Color.White * colorAlpha, effects: fx);
            }
        }

        static Transform _tmpXform = new Transform();

        private static void DrawHull(SpriteBatch spriteBatch, Entity ship, Entity hullEntity, ref Matrix shipMatrix)
        {
            var xform = ship.GetComponent<Transform>();
            var shipComponent = ship.GetComponent<ShipComponent>();
            var basedecal = shipComponent.Ship.BaseDecalColor.ToVector4();
            var trimdecal = shipComponent.Ship.TrimDecalColor.ToVector4();

            var hullComponent = hullEntity.GetComponent<HullComponent>();
            var sprite = hullEntity.GetComponent<SpriteComponent>();
            var hull = hullComponent.Part;
            var color = hull.Color;
            switch (hull.ColorType)
            {
                case HullColorType.BaseDecal:
                    color = new Color(basedecal * color.ToVector4());
                    break;
                case HullColorType.TrimDecal:
                    color = new Color(trimdecal * color.ToVector4());
                    break;
            }

            _tmpXform.CopyValuesFrom(hull.Transform);

            SpriteEffects fx = SpriteEffects.None;
            if (_tmpXform.Scale.X < 0)
            {
                fx |= SpriteEffects.FlipHorizontally;
                _tmpXform.Scale *= new Vector2(-1, 1);

            }
            if (_tmpXform.Scale.Y < 0)
            {
                fx |= SpriteEffects.FlipVertically;
                _tmpXform.Scale *= new Vector2(1, -1);
            }

            sprite.Draw(batch: spriteBatch, matrix: _tmpXform.Matrix * shipMatrix, color:color, effects: fx);
        }
    }
}
