using Artemis;
using FellSky.Systems.SceneGraphRenderers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public class ShipPartRendererComponent<T> : ISceneGraphRenderableComponent<T>
        where T : ISceneGraphRenderer
    {
        public virtual void Render(T renderer, SpriteBatch batch, Entity partEntity, ref Matrix parentMatrix)
        {
            var partComponent = partEntity.GetComponent<IShipPartComponent>();
            var color = partEntity.GetComponent<ColorComponent>()?.Color ?? partComponent.Part.Color;
            var sprite = partEntity.GetComponent<SpriteComponent>();
            var xform = partEntity.GetComponent<Transform>();
            SpriteEffects fx;

            var newTransform = xform.AdjustForFlipping(out fx);
            sprite.Draw(batch: batch, matrix: newTransform.Matrix * parentMatrix, color: color, effects: fx);
        }
    }
}
