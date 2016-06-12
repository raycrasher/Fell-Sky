using Artemis;
using Artemis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public class SceneGraphComponent: IComponent
    {
        public Entity Parent;
        public readonly List<Entity> Children = new List<Entity>();
    }
}
