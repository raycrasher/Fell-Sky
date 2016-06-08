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
    public class LightRenderableComponent : ISceneGraphRenderableComponent<StandardShipRenderer>
    {
        static ITimerService _timer;

        public void Render(StandardShipRenderer renderer, SpriteBatch spriteBatch, Entity lightEntity, ref Matrix parentMatrix)
        {
            _timer = _timer ?? ServiceLocator.Instance.GetService<ITimerService>();
            var lightComponent = lightEntity.GetComponent<NavLightComponent>();
            var light = lightComponent.Part;
            var theta = MathHelper.ToRadians(_timer.LastFrameUpdateTime.TotalGameTime.Milliseconds + lightComponent.Ship.GetHashCode());

            var sprite = lightEntity.GetComponent<SpriteComponent>();
            var xform = lightEntity.GetComponent<Transform>();
            float alpha = MathHelper.Clamp((float)(light.Amplitude * Math.Sin(theta * light.Frequency + light.PhaseShift) + light.VerticalShift), 0f, 1f);

            SpriteEffects fx;
            var newXform = xform.AdjustForFlipping(out fx);

            sprite.Draw(spriteBatch, newXform.Matrix * parentMatrix, light.Color * alpha, fx);
        }
    }
}
