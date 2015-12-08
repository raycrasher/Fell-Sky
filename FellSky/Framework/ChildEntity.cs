using Artemis;
using Artemis.Interface;
using Microsoft.Xna.Framework;

namespace FellSky.Framework
{
    public class ChildEntity: IComponent
    {
        public ChildEntity() { }

        public ChildEntity(Entity parent)
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
                    var parentchildcomponent = Parent.GetComponent<ChildEntity>();
                    if (parentchildcomponent != null)
                        matrix = parentchildcomponent.ParentWorldMatrix * matrix;
                }
                return matrix;
            }
        }
    }
}
