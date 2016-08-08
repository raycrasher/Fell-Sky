using Artemis;
using FellSky.Components;
using FellSky.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Editor.Systems
{
    public class AnimationEditorSpriteRendererSystem: Artemis.System.EntitySystem
    {
        private SpriteBatch _batch;

        public Color SpriteColor { get; set; } = Color.Magenta;

        public AnimationEditorSpriteRendererSystem()
            : base(Aspect.All(typeof(AnimationEditorComponent), typeof(AnimationComponent), typeof(SpriteComponent)))
        {
            _batch = ServiceLocator.Instance.GetService<SpriteBatch>();
        }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            _batch.Begin(transformMatrix: EntityWorld.GetActiveCamera().GetViewMatrix(1.0f));
            foreach(var entity in entities.Values)
            {
                var animation = entity.GetComponent<AnimationEditorComponent>();
                var sprite = entity.GetComponent<SpriteComponent>();
                Matrix matrix;
                entity.GetWorldMatrix(out matrix);
                matrix = animation.Transform.Matrix * matrix;

                sprite.Draw(_batch, matrix, SpriteColor);
            }
            _batch.End();
        }
    }
}
