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
    public class StandardShipSpriteRenderable : ISceneGraphRenderableComponent<StandardShipRenderer>
    {
        public void Render(StandardShipRenderer renderer, SpriteBatch batch, Entity entity, ref Matrix parentMatrix)
        {
            
        }
    }

}
