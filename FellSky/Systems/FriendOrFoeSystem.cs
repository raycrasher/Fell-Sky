using Artemis;
using FellSky.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Systems
{
    public class FriendOrFoeSystem: Artemis.System.EntitySystem
    {
        public FriendOrFoeSystem()
            : base(Aspect.All(typeof(IFFComponent)))
        {
        }

        public override void OnAdded(Entity entity)
        {
            
        }

        public override void OnChange(Entity entity)
        {
            
        }

        public override void OnRemoved(Entity entity)
        {
            
        }
    }
}
