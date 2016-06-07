using Artemis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FellSky.Components;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FellSky.Systems.SceneGraphRenderers;

namespace FellSky.Systems
{
    public class SceneGraphRendererSystem<T>: Artemis.System.EntitySystem
        where T : ISceneGraphRenderer
    {
        private SpriteBatch _batch;

        public SceneGraphRendererSystem(T renderer)
            : base(Aspect.All(typeof(SceneGraphRenderRoot<T>)))
        {
            _batch = Services.ServiceLocator.Instance.GetService<SpriteBatch>();
            Renderer = renderer;
        }

        public T Renderer { get; private set; }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            Renderer.Begin(EntityWorld, _batch, entities.Values);
            foreach (var root in entities.Values)
            {
                var matrix = Matrix.Identity;
                Render(root, ref matrix);
                
            }
            _batch.End();
        }

        private void Render(Entity entity, ref Matrix parentMatrix)
        {
            var sgRenderable = entity.GetComponent<ISceneGraphRenderableComponent<T>>();
            if (sgRenderable != null)
                sgRenderable.Render(Renderer, _batch, entity, ref parentMatrix);
            var sgComponent = entity.GetComponent<SceneGraphComponent>();
            if (sgComponent != null)
            {
                var matrix = (entity.GetComponent<Transform>()?.Matrix ?? Matrix.Identity) * parentMatrix;
                foreach (var child in sgComponent.Children)
                {
                    Render(child, ref matrix);
                }
            }
        }
    }
}
