using Artemis;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Systems.SceneGraphRenderers
{
    public interface ISceneGraphRenderer
    {
        void Begin(EntityWorld world, SpriteBatch batch, ICollection<Entity> root);
    }
}
