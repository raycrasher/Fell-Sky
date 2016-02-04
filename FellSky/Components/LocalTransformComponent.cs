using Artemis;
using Artemis.Interface;
using Microsoft.Xna.Framework;

namespace FellSky.Components
{
    public class LocalTransformComponent: IComponent
    {
        public LocalTransformComponent() { }

        public LocalTransformComponent(Entity parent)
        {
            Parent = parent;
        }

        public Entity Parent { get; set; }

        public Matrix ParentWorldMatrix {
            get
            {
                var matrix = Matrix.Identity;
                if (Parent != null)
                {
                    var xform = Parent.GetComponent<Transform>();
                    if (xform != null)
                        matrix = xform.GetMatrix();
                    var parentchildcomponent = Parent.GetComponent<LocalTransformComponent>();
                    if (parentchildcomponent != null)
                        matrix = parentchildcomponent.ParentWorldMatrix * matrix;
                }
                return matrix;
            }
        }
    }
}
