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
    public class SceneGraphRenderableComponent<T> : ISceneGraphRenderableComponent<T>
        where T : ISceneGraphRenderer
    { }

    public interface ISceneGraphRenderableComponent<T> : IComponent
        where T : ISceneGraphRenderer
    {
    }

    public class SceneGraphRenderRoot<T>: IComponent
        where T : ISceneGraphRenderer
    {
    }
}
