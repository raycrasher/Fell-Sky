using System.Collections.Generic;
using Artemis;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FellSky.Components;
using FellSky.Framework;

namespace FellSky.Components
{
    [Artemis.Attributes.ArtemisEntitySystem(ExecutionType = Artemis.Manager.ExecutionType.Synchronous, GameLoopType = Artemis.Manager.GameLoopType.Draw, Layer = 11 )]
    public class BoundingBoxRendererSystem : Artemis.System.EntityComponentProcessingSystem<Transform, DrawBoundingBoxComponent>
    {
        private SpriteBatch _spriteBatch;
        private CameraComponent _camera;

        public string CameraTag { get; set; }

        public BoundingBoxRendererSystem(SpriteBatch spriteBatch, string cameraTag)
        {
            CameraTag = cameraTag;
            _spriteBatch = spriteBatch;
        }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            _camera = EntityWorld.GetCamera(CameraTag);
            if (_camera == null) return;
            _spriteBatch.Begin(transformMatrix: _camera?.GetViewMatrix(1.0f));            
            base.ProcessEntities(entities);
            _spriteBatch.End();
        }

        public override void Process(Entity entity, Transform xform, DrawBoundingBoxComponent box)
        {
            if (!box.IsEnabled) return;

            var matrix = entity.GetWorldMatrix();
            //var child = entity.GetComponent<ChildEntityComponent>();
            //if (child != null)
            //    matrix = child.ParentWorldMatrix * matrix;

            var p1 = new Vector2(box.BoundingBox.Left, box.BoundingBox.Top);
            var p2 = new Vector2(box.BoundingBox.Right, box.BoundingBox.Top);
            var p3 = new Vector2(box.BoundingBox.Right, box.BoundingBox.Bottom);
            var p4 = new Vector2(box.BoundingBox.Left, box.BoundingBox.Bottom);

            Vector2.Transform(ref p1, ref matrix, out p1);
            Vector2.Transform(ref p2, ref matrix, out p2);
            Vector2.Transform(ref p3, ref matrix, out p3);
            Vector2.Transform(ref p4, ref matrix, out p4);

            _spriteBatch.DrawLine(p1, p2, box.Color);
            _spriteBatch.DrawLine(p2, p3, box.Color);
            _spriteBatch.DrawLine(p3, p4, box.Color);
            _spriteBatch.DrawLine(p4, p1, box.Color);
        }
    }
}
