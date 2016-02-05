using Artemis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public class CollisionComponent : IComponent
    {
        public bool HandleAfterCollision { get; internal set; }
        public bool HandleBeforeCollision { get; internal set; }
        public bool HandleCollision { get; internal set; }
        public bool HandleOnSeparation { get; internal set; }
    }
}
