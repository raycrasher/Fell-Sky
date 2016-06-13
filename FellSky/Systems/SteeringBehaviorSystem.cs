using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using FellSky.Components;
using Microsoft.Xna.Framework;
using FellSky.Framework.SteeringBehaviors;

namespace FellSky.Systems
{
    public class SteeringBehaviorSystem: Artemis.System.EntitySystem
    {
        public Artemis.Utils.Bag<Entity> Obstacles => EntityWorld.EntityManager.GetEntities(Aspect.All(typeof(SteeringObstacleComponent)));

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            foreach(var entity in entities.Values)
            {
                Array.Clear(NewDangerMap, 0, SteeringMapSize);
                Array.Clear(NewInterestMap, 0, SteeringMapSize);

                var steer = entity.GetComponent<SteeringBehaviorComponent>();

                foreach (var behavior in steer.Behaviors)
                    behavior.Update(entity, NewDangerMap, NewInterestMap);

                PerformHysteresis(steer.DangerMap, NewDangerMap);
                PerformHysteresis(steer.InterestMap, NewInterestMap);

                steer.DesiredMovementVector = CalculateMovementVector(steer.DangerMap, steer.InterestMap);
            }
        }

        public const int SteeringMapSize = 8;

        float[] NewDangerMap = new float[SteeringMapSize];
        float[] NewInterestMap = new float[SteeringMapSize];

        public override void OnAdded(Entity entity)
        {
            base.OnAdded(entity);
            var steer = entity.GetComponent<SteeringBehaviorComponent>();
            Array.Clear(steer.DangerMap, 0, SteeringMapSize);
            Array.Clear(steer.InterestMap, 0, SteeringMapSize);
        }

        private Vector2 CalculateMovementVector(float[] dangerMap, float[] interestMap)
        {
            throw new NotImplementedException();
        }

        private void PerformHysteresis(float[] oldMap, float[] newMap)
        {
            for (int i = 0; i < SteeringMapSize; i++)
            {
                oldMap[i] = (oldMap[i] + newMap[i]) / 2;
            }
        }
    }
}
