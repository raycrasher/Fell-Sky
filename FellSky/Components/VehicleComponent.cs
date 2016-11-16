using Artemis;
using Artemis.Interface;
using SharpSteer2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public class VehicleComponent: SimpleVehicle, IComponent
    {
        public Entity Entity; // back-reference
    }
}
