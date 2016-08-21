using Artemis;
using FellSky.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Systems
{
    public class BeamSystem: Artemis.System.EntitySystem
    {
        public BeamSystem()
            : base(Aspect.All(typeof(BeamComponent)))
        { }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            foreach(var beamEntity in entities.Values)
            {
                var beamComponent = beamEntity.GetComponent<BeamComponent>();

            }
        }
    }
}
