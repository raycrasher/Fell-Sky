using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Framework.SteeringBehaviors
{
    public abstract class SteeringBehavior
    {
        public abstract void Update(Artemis.Entity entity, float[] dangerMap, float[] interestMap);

        protected static void WriteCurve(float[] map, float direction, float value, float range)
        {

        }
    }
}
