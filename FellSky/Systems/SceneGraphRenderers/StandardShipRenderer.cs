using Artemis;
using FellSky.Components;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace FellSky.Systems.SceneGraphRenderers
{
    public class StandardShipRenderer : ISceneGraphRenderer
    {
        public void Begin(EntityWorld world, SpriteBatch batch, ICollection<Entity> entities)
        {
            batch.Begin(transformMatrix: world.GetActiveCamera().GetViewMatrix(1.0f));
        }
    }
}
