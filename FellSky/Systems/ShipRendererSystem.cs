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
using FellSky.Game.Ships;

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
            var camera = EntityWorld.GetActiveCamera();
            if (camera == null) return;
            _matrix = camera.GetViewMatrix(1.0f);
            _spriteBatch.Begin(SpriteSortMode.Deferred, blendState: BlendState.AlphaBlend, transformMatrix: _matrix, samplerState: SamplerState.AnisotropicClamp);

            foreach (var entity in entities.Values)
            {
                DrawShip(_spriteBatch, entity, Matrix.Identity);
            }

            _spriteBatch.End();
        }

        public static void DrawShip(SpriteBatch spriteBatch, Entity ship, Matrix matrix)
        {
            var shipComponent = (IShipEditorEditableComponent) ship.GetComponent<ShipComponent>() ?? ship.GetComponent<ShipPartGroupComponent>();
            var xform = ship.GetComponent<Transform>();

            TempXform.CopyValuesFrom(xform);
            TempXform.Scale = new Vector2(Math.Abs(xform.Scale.X), Math.Abs(xform.Scale.Y));
            TempXform.Origin = Vector2.Zero;

            TempXform.Rotation += ship.GetComponent<TurretComponent>()?.Rotation ?? 0;

            var shipMatrix = TempXform.Matrix * matrix;

            foreach(var item in shipComponent.PartEntities)
            {
                if (item.Entity.HasComponent<HullComponent>())
                {
                    DrawHull(spriteBatch, shipComponent, ship, item.Entity, ref shipMatrix);

                    var hardpoint = item.Entity.GetComponent<HardpointComponent>();
                    if (hardpoint!=null && hardpoint.InstalledEntity!=null && hardpoint.InstalledEntity.HasComponent<ShipPartGroupComponent>())
                    {
                        DrawShip(spriteBatch, hardpoint.InstalledEntity, shipMatrix);
                    }
                }
                else if (item.Entity.HasComponent<ThrusterComponent>())
                    DrawThruster(spriteBatch, shipComponent, ship, item.Entity, ref shipMatrix);
                else if (item.Entity.HasComponent<NavLightComponent>())
                    DrawLight(spriteBatch, shipComponent, ship, item.Entity, ref shipMatrix);
            }
        }

        private static void DrawLight(SpriteBatch spriteBatch, IShipEditorEditableComponent shipComponent, Entity ship, Entity lightEntity, ref Matrix shipMatrix)
        {
            var lightComponent = lightEntity.GetComponent<NavLightComponent>();
            var light = lightComponent.Part;
            var theta = MathHelper.ToRadians(_timer.LastFrameUpdateTime.TotalGameTime.Milliseconds + ship.GetHashCode());

            var sprite = lightEntity.GetComponent<SpriteComponent>();
            var xform = lightEntity.GetComponent<Transform>();
            float alpha = MathHelper.Clamp((float)(light.Amplitude * Math.Sin(theta * light.Frequency + light.PhaseShift) + light.VerticalShift), 0f, 1f);

            var fx = UpdateTempXform(xform);

            sprite.Draw(spriteBatch, TempXform.Matrix * shipMatrix, light.Color * alpha, fx);
        }

        private static void DrawThruster(SpriteBatch spriteBatch, IShipEditorEditableComponent shipComponent, Entity ship, Entity thrusterEntity, ref Matrix shipMatrix)
        {            
            var thrusterComponent = thrusterEntity.GetComponent<ThrusterComponent>();
            if (thrusterComponent.ThrustPercentage > 0 || thrusterComponent.Part.IsIdleModeOnZeroThrust)
            {
                var sprite = thrusterEntity.GetComponent<SpriteComponent>();
                var thruster = thrusterComponent.Part;

                var fx = UpdateTempXform(thruster.Transform);

                float colorAlpha = 0; 
                
                if (thrusterComponent.Part.IsIdleModeOnZeroThrust)
                {
                    TempXform.Scale *= new Vector2(MathHelper.Clamp(thrusterComponent.ThrustPercentage, 0.3f, 1), 1);
                    colorAlpha = MathHelper.Clamp(thrusterComponent.ThrustPercentage, 0.6f, 1);
                } else
                {
                    TempXform.Scale *= new Vector2(MathHelper.Clamp(thrusterComponent.ThrustPercentage, 0, 1), 1);
                    colorAlpha = thrusterComponent.ThrustPercentage;
                }

                if (!thrusterEntity.HasComponent<EditorComponent>())
                {
                    // do thruster graphic wobble
                    var time = MathHelper.ToRadians(thrusterComponent.GetHashCode() + _timer.LastFrameUpdateTime.TotalGameTime.Milliseconds);
                    var amount = (float)Math.Sin((time % MathHelper.Pi * 2) * 1f);
                    TempXform.Scale += new Vector2(amount * 0.01f, amount * 0.01f);
                }
                sprite.Draw(batch: spriteBatch, matrix: TempXform.Matrix * shipMatrix, color: thruster.Color * colorAlpha, effects: fx);
                TempXform.Scale *= 0.8f;
                sprite.Draw(batch: spriteBatch, matrix: TempXform.Matrix * shipMatrix, color: Color.White * colorAlpha, effects: fx);
            }
        }

        static Transform TempXform = new Transform();

        private static void DrawHull(SpriteBatch spriteBatch, IShipEditorEditableComponent shipComponent, Entity ship, Entity hullEntity, ref Matrix shipMatrix)
        {
            var xform = ship.GetComponent<Transform>();
            var basedecal = shipComponent.Model.BaseDecalColor.ToVector4();
            var trimdecal = shipComponent.Model.TrimDecalColor.ToVector4();

            var hullComponent = hullEntity.GetComponent<HullComponent>();
            var sprite = hullEntity.GetComponent<SpriteComponent>();
            var hull = hullComponent.Part;
            var color = hullEntity.GetComponent<ColorComponent>()?.Color ?? hull.Color;
            switch (hull.ColorType)
            {
                case HullColorType.BaseDecal:
                    color = new Color(basedecal * color.ToVector4());
                    break;
                case HullColorType.TrimDecal:
                    color = new Color(trimdecal * color.ToVector4());
                    break;
            }

            if (color.A == 0) return;
            SpriteEffects fx = UpdateTempXform(hull.Transform);
            sprite.Draw(batch: spriteBatch, matrix: TempXform.Matrix * shipMatrix, color:color, effects: fx);
        }

        private static SpriteEffects UpdateTempXform(Transform xform)
        {
            SpriteEffects fx = SpriteEffects.None;
            TempXform.CopyValuesFrom(xform);
            if (TempXform.Scale.X < 0)
            {
                fx |= SpriteEffects.FlipHorizontally;
                TempXform.Scale *= new Vector2(-1, 1);

            }
            if (TempXform.Scale.Y < 0)
            {
                fx |= SpriteEffects.FlipVertically;
                TempXform.Scale *= new Vector2(1, -1);
            }
            return fx;
        }
    }
}
