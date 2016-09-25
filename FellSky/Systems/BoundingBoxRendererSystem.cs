using System.Collections.Generic;
using Artemis;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FellSky.Components;
using FellSky.Framework;
using FellSky.Services;

namespace FellSky.Systems
{
    public class BoundingBoxRendererSystem : Artemis.System.EntityComponentProcessingSystem<Transform, DrawBoundingBoxComponent, BoundingBoxComponent>
    {
        private SpriteBatch _spriteBatch;

        public BoundingBoxRendererSystem()
        {
            _spriteBatch = ServiceLocator.Instance.GetService<SpriteBatch>();
        }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            var camera = EntityWorld.TagManager.GetEntity(Constants.ActiveCameraTag).GetComponent<Camera>();
            if (camera == null) return;
            _spriteBatch.Begin(effect: camera.SpriteBatchBasicEffect);            
            base.ProcessEntities(entities);
            _spriteBatch.End();
        }

        public override void Process(Entity entity, Transform xform, DrawBoundingBoxComponent draw, BoundingBoxComponent box)
        {
            if (!draw.IsEnabled) return;

            Matrix matrix;
            entity.GetWorldMatrix(out matrix);
            //var child = entity.GetComponent<ChildEntityComponent>();
            //if (child != null)
            //    matrix = child.ParentWorldMatrix * matrix;

            var p1 = new Vector2(box.Box.Left, box.Box.Top);
            var p2 = new Vector2(box.Box.Right, box.Box.Top);
            var p3 = new Vector2(box.Box.Right, box.Box.Bottom);
            var p4 = new Vector2(box.Box.Left, box.Box.Bottom);

            Vector2.Transform(ref p1, ref matrix, out p1);
            Vector2.Transform(ref p2, ref matrix, out p2);
            Vector2.Transform(ref p3, ref matrix, out p3);
            Vector2.Transform(ref p4, ref matrix, out p4);

            _spriteBatch.DrawLine(p1, p2, draw.Color);
            _spriteBatch.DrawLine(p2, p3, draw.Color);
            _spriteBatch.DrawLine(p3, p4, draw.Color);
            _spriteBatch.DrawLine(p4, p1, draw.Color);
        }
    }
}
