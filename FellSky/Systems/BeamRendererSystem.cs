using Artemis;
using FellSky.Components;
using FellSky.Services;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Systems
{
    public class BeamRendererSystem: Artemis.System.EntitySystem
    {
        private SpriteBatch _batch;

        public BeamRendererSystem()
            : base(Aspect.All(typeof(Transform), typeof(BeamComponent)))
        {
            _batch = ServiceLocator.Instance.GetService<SpriteBatch>();
        }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            _batch.Begin(transformMatrix: EntityWorld.GetActiveCamera().GetViewMatrix(1.0f));
            foreach(var item in entities.Values)
            {
                var xform = item.GetComponent<Transform>();
                var beam = item.GetComponent<BeamComponent>();
            }
            _batch.End();
        }
    }
}
