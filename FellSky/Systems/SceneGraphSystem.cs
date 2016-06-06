using Artemis;
using FellSky.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Systems
{
    public class SceneGraphSystem: Artemis.System.EntitySystem
    {
        public SceneGraphSystem()
            : base(Aspect.All(typeof(SceneGraphComponent)))
        { }

        public override void Process()
        {
        }

        public override void OnRemoved(Entity entity)
        {
            if (entity.DeletingState)
            {
                var component = entity.GetComponent<SceneGraphComponent>();
                foreach (var child in component.Children)
                    child.Delete();
            }
        }
    }
}
