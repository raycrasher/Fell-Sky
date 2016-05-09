using FellSky.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;

namespace FellSky.Editor.Systems
{
    class ShowThrusterTrailsOverrideSystem : Artemis.System.EntityComponentProcessingSystem<ThrusterComponent>
    {
        public override void Process(Entity entity, ThrusterComponent thruster)
        {
            thruster.ThrustPercentage = 1;
        }
    }
}
