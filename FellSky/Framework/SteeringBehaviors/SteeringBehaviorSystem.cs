using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using Microsoft.Xna.Framework;
using FellSky.Framework.SteeringBehaviors;

namespace FellSky.Framework.SteeringBehaviors
{
    class SteeringBehaviorSystem : Artemis.System.EntityComponentProcessingSystem<SteeringBehavior>
    {
        public const int SteeringMapSize = 8;

        float[] NewDangerMap = new float[SteeringMapSize];
        float[] NewInterestMap = new float[SteeringMapSize];

        public override void OnAdded(Entity entity)
        {
            base.OnAdded(entity);
            var steer = entity.GetComponent<SteeringBehavior>();
            Array.Clear(steer.DangerMap, 0, SteeringMapSize);
            Array.Clear(steer.InterestMap, 0, SteeringMapSize);
        }

        public override void Process(Entity entity, SteeringBehavior steer)
        {
            Array.Clear(NewDangerMap, 0, SteeringMapSize);
            Array.Clear(NewInterestMap, 0, SteeringMapSize);

            foreach (var behavior in steer.Behaviors)
                behavior.Update(entity, NewDangerMap, NewInterestMap);

            PerformHysteresis(steer.DangerMap, NewDangerMap);
            PerformHysteresis(steer.InterestMap, NewInterestMap);

            steer.DesiredMovementVector = CalculateMovementVector(steer.DangerMap, steer.InterestMap);
        }

        private Vector2 CalculateMovementVector(float[] dangerMap, float[] interestMap)
        {
            throw new NotImplementedException();
        }

        private void PerformHysteresis(float[] oldMap, float[] newMap)
        {
            for(int i = 0; i < SteeringMapSize; i++)
            {
                oldMap[i] = (oldMap[i] + newMap[i]) / 2;
            }
        }
    }
}
