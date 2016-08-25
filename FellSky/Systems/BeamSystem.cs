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
                beamComponent.Age += (EntityWorld.Delta / 1000f);

                if (beamComponent.IsPowered)
                {
                    if (beamComponent.Beam.IntensityFadeInTime > 0)
                    {
                        beamComponent.Intensity += (EntityWorld.Delta / beamComponent.Beam.IntensityFadeInTime) / 1000f;
                        if (beamComponent.Intensity > 1)
                            beamComponent.Intensity = 1;
                    }
                    else
                    {
                        beamComponent.Intensity = 1;
                    }
                }
                else
                {
                    if (beamComponent.Beam.IntensityFadeOutTime > 0)
                    {
                        beamComponent.Intensity -= (EntityWorld.Delta / beamComponent.Beam.IntensityFadeOutTime) / 1000f;
                        if (beamComponent.Intensity < 0)
                            beamComponent.Intensity = 0;
                    }
                    else
                    {
                        beamComponent.Intensity = 0;
                    }

                    if(beamComponent.Intensity <= 0)
                    {
                        beamEntity.Delete();
                    }
                }
            }
        }
    }
}
