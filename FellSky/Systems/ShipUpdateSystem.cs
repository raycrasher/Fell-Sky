using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using FellSky.Components;

namespace FellSky.Systems
{
    public class ShipUpdateSystem: Artemis.System.ParallelEntityProcessingSystem
    {
        public ShipUpdateSystem()
            : base (Aspect.All(typeof(ShipComponent)))
        { }


        /// <summary>
        /// Event handler. Also removes all child entities of the ship.
        /// </summary>
        /// <param name="entity"></param>
        public override void OnRemoved(Entity entity)
        {
            var shipComponent = entity.GetComponent<ShipComponent>();
            if (shipComponent != null)
            {
                foreach (var child in shipComponent.ChildEntities)
                {
                    child.Delete();
                }
            }

            base.OnRemoved(entity);
        }

        public override void Process(Entity entity)
        {
            var shipComponent = entity.GetComponent<ShipComponent>();
        }

        private void UpdateThrusters(Entity ship, IList<Entity> thrusterEntities)
        {
            
        }
    }
}
