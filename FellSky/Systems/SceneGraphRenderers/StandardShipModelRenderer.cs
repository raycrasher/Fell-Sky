using Artemis;
using FellSky.Components;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using FellSky.Services;

namespace FellSky.Systems.SceneGraphRenderers
{
    public class StandardShipModelRenderer : ISceneGraphRenderer
    {
        private RasterizerState _rasterizerState;
        private RasterizerState _lastRasterizerState;
        private SpriteBatch _batch;
        private EntityWorld _world;
        private ulong _totalTime = 0;

        public StandardShipModelRenderer()
        {
            _rasterizerState = new RasterizerState();
            _rasterizerState.CullMode = CullMode.None;
        }

        public void Begin(EntityWorld world, SpriteBatch batch)
        {
            _totalTime += (ulong)world.Delta;
            _batch = batch;
            _world = world;
            _lastRasterizerState = batch.GraphicsDevice.RasterizerState;
            batch.GraphicsDevice.RasterizerState = _rasterizerState;
            batch.Begin(transformMatrix: world.GetActiveCamera().GetViewMatrix(1.0f));
        }

        public void End()
        {
            _batch.End();
            _batch.GraphicsDevice.RasterizerState = _lastRasterizerState;
        }

        public void BeginRoot(EntityWorld world, SpriteBatch batch, Entity root)
        {
            
        }

        public void Render(Entity root, Entity entity, ref Matrix parentMatrix)
        {
            if (entity.HasComponent<HullComponent>())
                RenderHull(root, entity, ref parentMatrix);
            else if (entity.HasComponent<ThrusterComponent>())
                RenderThruster(root, entity, ref parentMatrix);
            else if (entity.HasComponent<NavLightComponent>())
                RenderLight(root, entity, ref parentMatrix);
        }

        private void RenderHull(Entity root, Entity partEntity, ref Matrix parentMatrix)
        {
            var partComponent = partEntity.GetComponent<IShipPartComponent>();
            var color = partEntity.GetComponent<ColorComponent>()?.Color ?? partComponent.Part.Color;
            var sprite = partEntity.GetComponent<SpriteComponent>();
            var xform = partEntity.GetComponent<Transform>();
            var hull = ((HullComponent)partComponent).Part;
            var shipComponent = root.GetComponent<ShipModelComponent>();
            if(shipComponent != null)
            {
                switch (hull.ColorType)
                {
                    case Game.Ships.Parts.HullColorType.Base:
                        color = new Color(color.ToVector4() * shipComponent.BaseDecalColor.ToVector4());
                        break;
                    case Game.Ships.Parts.HullColorType.Trim:
                        color = new Color(color.ToVector4() * shipComponent.TrimDecalColor.ToVector4());
                        break;
                }
            }

            SpriteEffects fx;

            var newTransform = xform.AdjustForFlipping(out fx);
            sprite.Draw(batch: _batch, matrix: newTransform.Matrix * parentMatrix, color: color, effects: fx);
        }

        private void RenderThruster(Entity root, Entity thrusterEntity, ref Matrix parentMatrix)
        {
            var thrusterComponent = thrusterEntity.GetComponent<ThrusterComponent>();
            if (thrusterComponent.ThrustPercentage > 0 || thrusterComponent.Part.IsIdleModeOnZeroThrust)
            {
                var sprite = thrusterEntity.GetComponent<SpriteComponent>();
                var thruster = thrusterComponent.Part;

                SpriteEffects fx;
                var tempXform = thruster.Transform.AdjustForFlipping(out fx);

                float colorAlpha = 0;

                if (thrusterComponent.Part.IsIdleModeOnZeroThrust)
                {
                    tempXform.Scale *= new Vector2(MathHelper.Clamp(thrusterComponent.ThrustPercentage, 0.3f, 1), 1);
                    colorAlpha = MathHelper.Clamp(thrusterComponent.ThrustPercentage, 0.6f, 1);
                }
                else
                {
                    tempXform.Scale *= new Vector2(MathHelper.Clamp(thrusterComponent.ThrustPercentage, 0, 1), 1);
                    colorAlpha = thrusterComponent.ThrustPercentage;
                }

                if (!thrusterEntity.HasComponent<EditorComponent>())
                {
                    // do thruster graphic wobble
                    var time = MathHelper.ToRadians((float)thrusterComponent.GetHashCode() + _totalTime);
                    var amount = (float)Math.Sin(((time * 1.5f) % MathHelper.Pi) * 1f);
                    tempXform.Scale += new Vector2(amount * 0.05f, amount * 0.03f);
                }
                sprite.Draw(batch: _batch, matrix: tempXform.Matrix * parentMatrix, color: thruster.Color * colorAlpha, effects: fx);
                tempXform.Scale *= 0.8f;
                sprite.Draw(batch: _batch, matrix: tempXform.Matrix * parentMatrix, color: Color.White * colorAlpha, effects: fx);
            }
        }

        private void RenderLight(Entity root, Entity lightEntity, ref Matrix parentMatrix)
        {
            var lightComponent = lightEntity.GetComponent<NavLightComponent>();
            var light = lightComponent.Part;
            var theta = MathHelper.ToRadians((float)lightComponent.Ship.GetHashCode() + _totalTime);

            var sprite = lightEntity.GetComponent<SpriteComponent>();
            var xform = lightEntity.GetComponent<Transform>();
            float alpha = MathHelper.Clamp((float)(light.Amplitude * Math.Sin(theta * light.Frequency + light.PhaseShift) + light.VerticalShift), 0f, 1f);

            SpriteEffects fx;
            var newXform = xform.AdjustForFlipping(out fx);

            sprite.Draw(_batch, newXform.Matrix * parentMatrix, light.Color * alpha, fx);
        }


    }
}
