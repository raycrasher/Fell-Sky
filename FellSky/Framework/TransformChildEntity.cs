using Artemis;
using Artemis.Interface;
using Microsoft.Xna.Framework;

namespace FellSky.Framework
{
    public class TransformChildEntity: IComponent
    {
        public TransformChildEntity() { }

        public TransformChildEntity(Entity parent)
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
                    var parentchildcomponent = Parent.GetComponent<TransformChildEntity>();
                    if (parentchildcomponent != null)
                        matrix = parentchildcomponent.ParentWorldMatrix * matrix;
                }
                return matrix;
            }
        }
    }
}
