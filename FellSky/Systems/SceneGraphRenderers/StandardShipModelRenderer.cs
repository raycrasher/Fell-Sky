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
using FellSky.Framework;

namespace FellSky.Systems.SceneGraphRenderers
{
    public class StandardShipModelRenderer : ISceneGraphRenderer
    {
        private RasterizerState _rasterizerState;
        private RasterizerState _lastRasterizerState;
        private GraphicsDevice _device;
        //private SpriteBatch _batch;
        private FastSpriteBatch _batch;
        private EntityWorld _world;
        private BasicEffect _shipEffect;
        private ulong _totalTime = 0;

        public StandardShipModelRenderer()
        {
            _rasterizerState = new RasterizerState();
            var content = ServiceLocator.Instance.GetService<Microsoft.Xna.Framework.Content.ContentManager>();
            _rasterizerState = RasterizerState.CullNone;
            _device = ServiceLocator.Instance.GetService<GraphicsDevice>();
            _batch = new FastSpriteBatch();
            _shipEffect = new BasicEffect(_device);
            _shipEffect.TextureEnabled = true;            
            _shipEffect.Texture = content.Load<Texture2D>("Textures/hulls");
            _shipEffect.VertexColorEnabled = true;
            var projectionDivisor = 1f;
            _shipEffect.Projection = Matrix.CreateOrthographic(_device.Viewport.Width / projectionDivisor, _device.Viewport.Height / projectionDivisor, 1.0f, 10000.0f);
        }

        public void Begin(EntityWorld world)
        {
            _totalTime += (ulong)world.Delta;
            _world = world;
            _lastRasterizerState = _device.RasterizerState;
            
            //batch.GraphicsDevice.RasterizerState = _rasterizerState;
            var _camera = world.GetActiveCamera();
            var inverse = new Vector2(1, -1) * 0.01f;
            _shipEffect.View = Matrix.CreateLookAt(new Vector3(_camera.Transform.Position * inverse, 100), new Vector3(_camera.Transform.Position * inverse, 0), Vector3.UnitY) * Matrix.CreateScale(_camera.Zoom);
            _batch.Reset();
        }

        public void End()
        {
            _device.DepthStencilState = DepthStencilState.None;
            _device.SamplerStates[0] = SamplerState.AnisotropicClamp;
            _device.RasterizerState = _rasterizerState;
            _batch.Render(_device, _shipEffect);
            _device.RasterizerState = _lastRasterizerState;
            //_batch.End();
            //_batch.GraphicsDevice.RasterizerState = _lastRasterizerState;
        }

        public void BeginRoot(EntityWorld world, Entity root)
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

            Matrix matrix;
            //var fx = xform.AdjustForFlipping(out matrix);

            //matrix *= parentMatrix;
            matrix = xform.Matrix * parentMatrix;
            //_batch.Draw(sprite, ref matrix, color, flip: fx);
            _batch.Draw(sprite, ref matrix, color);

            //sprite.Draw(batch: _batch, matrix: newTransform.Matrix * parentMatrix, color: color, effects: fx);
        }

        private void RenderThruster(Entity root, Entity thrusterEntity, ref Matrix parentMatrix)
        {
            var thrusterComponent = thrusterEntity.GetComponent<ThrusterComponent>();
            if (thrusterComponent.ThrustPercentage > 0 || thrusterComponent.Part.IsIdleModeOnZeroThrust)
            {
                var sprite = thrusterEntity.GetComponent<SpriteComponent>();
                var thruster = thrusterComponent.Part;
                var xform = thruster.Transform;
                Vector2 scale = xform.Scale;

                float colorAlpha = 0;

                if (thrusterComponent.Part.IsIdleModeOnZeroThrust)
                {
                    scale *= new Vector2(MathHelper.Clamp(thrusterComponent.ThrustPercentage, 0.3f, 1), 1);
                    colorAlpha = MathHelper.Clamp(thrusterComponent.ThrustPercentage, 0.6f, 1);
                }
                else
                {
                    scale *= new Vector2(MathHelper.Clamp(thrusterComponent.ThrustPercentage, 0, 1), 1);
                    colorAlpha = thrusterComponent.ThrustPercentage;
                }

                if (!thrusterEntity.HasComponent<EditorComponent>())
                {
                    // do thruster graphic wobble
                    var time = MathHelper.ToRadians((float)thrusterComponent.GetHashCode() + _totalTime);
                    var amount = (float)Math.Sin(((time * 1.5f) % MathHelper.Pi) * 1f);
                    scale += new Vector2(amount * 0.05f, amount * 0.03f);
                }

                var matrix = UtilityExtensions.GetMatrix(xform.Position, xform.Rotation, scale, xform.Origin) * parentMatrix;
                _batch.Draw(sprite, ref matrix, thruster.Color * colorAlpha);
                //sprite.Draw(batch: _batch, matrix: tempXform.Matrix * parentMatrix, color: thruster.Color * colorAlpha, effects: fx);
                scale *= 0.8f;
                matrix = UtilityExtensions.GetMatrix(xform.Position, xform.Rotation, scale, xform.Origin) * parentMatrix;
                _batch.Draw(sprite, ref matrix, Color.White * colorAlpha);
                //sprite.Draw(batch: _batch, matrix: tempXform.Matrix * parentMatrix, color: Color.White * colorAlpha, effects: fx);
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

            Matrix matrix;
            var fx = xform.AdjustForFlipping(out matrix);

            //sprite.Draw(_batch, newXform.Matrix * parentMatrix, light.Color * alpha, fx);
        }


    }
}
