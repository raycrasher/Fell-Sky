using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Framework.SteeringBehaviors
{
    public interface ISteeringBehavior
    {
        void Update(Artemis.Entity entity, float[] dangerMap, float[] interestMap);
    }
}
