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
    public class BulletRendererSystem: Artemis.System.EntitySystem
    {
        private SpriteBatch _batch;

        public BulletRendererSystem()
            : base(Aspect.All(typeof(BasicBulletComponent), typeof(Transform), typeof(SpriteComponent)))
        {
            _batch = ServiceLocator.Instance.GetService<SpriteBatch>();
        }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            _batch.Begin(transformMatrix: EntityWorld.GetActiveCamera().GetViewMatrix(1.0f));
            foreach (var entity in entities.Values)
            {
                var bullet = entity.GetComponent<BasicBulletComponent>();
                var xform = entity.GetComponent<Transform>();
                var sprite = entity.GetComponent<SpriteComponent>();
                sprite.Draw(_batch, xform, bullet.Color * bullet.Alpha);
            }
            _batch.End();
        }
    }
}
