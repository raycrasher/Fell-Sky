using Artemis;
using FellSky.Components;
using FellSky.Services;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Systems
{
    public class MouseHoverSystem: Artemis.System.EntitySystem
    {
        private IMouseService _mouse;

        public MouseHoverSystem()
            : base(Aspect.All(typeof(Transform), typeof(BoundingBoxComponent), typeof(MouseHoverComponent)))
        {
            _mouse = ServiceLocator.Instance.GetService<IMouseService>();
        }


        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            var camera = EntityWorld.GetActiveCamera();
            var mousePos = camera.ScreenToCameraSpace(_mouse.ScreenPosition);

            foreach(var entity in entities.Values)
            {
                var box = entity.GetComponent<BoundingBoxComponent>().Box;
                var hover = entity.GetComponent<MouseHoverComponent>();
                var bigbox = box;
                Matrix matrix = Matrix.Identity;

                if (hover.UsePositionOnly)
                {
                    var transform = entity.GetComponent<Transform>();
                    var posMatrix = Matrix.CreateTranslation(new Vector3(transform.Position, 0));

                    Matrix parentMatrix = Matrix.Identity;
                    entity.GetParent()?.GetWorldMatrix(out parentMatrix);
                    matrix = posMatrix * parentMatrix;
                }
                else {
                    entity.GetWorldMatrix(out matrix);
                }


                Vector2 pos;
                var xform = Matrix.Invert(matrix);
                Vector2.Transform(ref mousePos, ref xform, out pos);
                hover.IsHover = box.Contains(pos);
            }
        }
    }
}
