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
        public Entity Parent { get; set; }
        public List<Entity> Children { get; } = new List<Entity>();
    }
}
