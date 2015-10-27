using Artemis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.EntityComponents
{
    public class SensorDetectableComponent: IComponent
    {
        public float InfraredEmission { get; set; }
        public float LightEmission { get; set; }
        public float RadioEmission { get; set; }
        public float IonEmission { get; set; }
        public float RadarAlbedo { get; set; }
        public float VisualAlbedo { get; set; }
    }
}
