using Artemis;
using Artemis.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FellSky.Systems.SceneGraphRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public interface ISceneGraphRenderableComponent<T> : IComponent
        where T : ISceneGraphRenderer
    {
        void Render(T renderer, SpriteBatch batch, Entity entity, ref Matrix parentMatrix);
    }
}
