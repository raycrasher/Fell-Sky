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

        public override void OnRemoved(Entity entity)
        {
            var parent = entity.GetParent();
            if (parent != null) parent.RemoveChild(entity);
            var component = entity.GetComponent<SceneGraphComponent>();
            foreach (var child in component.Children)
                child.Delete();

        }

    }
}
