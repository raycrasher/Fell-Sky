using Artemis;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace FellSky.Systems.SceneGraphRenderers
{
    public interface ISceneGraphRenderer
    {
        void Begin(EntityWorld world, SpriteBatch batch);
        void BeginRoot(EntityWorld world, SpriteBatch batch, Entity root);
        void Render(SpriteBatch batch, Entity root, Entity entity, ref Matrix parentMatrix);
        void End(EntityWorld entityWorld, SpriteBatch _batch);
    }
}
