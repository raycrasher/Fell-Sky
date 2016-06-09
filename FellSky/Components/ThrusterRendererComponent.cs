using FellSky.Systems.SceneGraphRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FellSky.Services;

namespace FellSky.Components
{
    public class ThrusterRendererComponent : ISceneGraphRenderableComponent<StandardShipRenderer>
    {
        static ITimerService Timer;

        public void Render(StandardShipRenderer renderer, SpriteBatch spriteBatch, Entity thrusterEntity, ref Matrix parentMatrix)
        {
            Timer = Timer ?? ServiceLocator.Instance.GetService<ITimerService>();
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
                    var time = MathHelper.ToRadians(thrusterComponent.GetHashCode() + Timer.LastFrameUpdateTime.TotalGameTime.Milliseconds);
                    var amount = (float)Math.Sin((time % MathHelper.Pi * 2) * 1f);
                    tempXform.Scale += new Vector2(amount * 0.01f, amount * 0.01f);
                }
                sprite.Draw(batch: spriteBatch, matrix: tempXform.Matrix * parentMatrix, color: thruster.Color * colorAlpha, effects: fx);
                tempXform.Scale *= 0.8f;
                sprite.Draw(batch: spriteBatch, matrix: tempXform.Matrix * parentMatrix, color: Color.White * colorAlpha, effects: fx);
            }
        }
    }
}
