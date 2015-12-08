using FellSky.Ships.Parts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Ships
{
    public class ThermalSubsystem: IShipSubsystem
    {
        public List<IHeatProducer> HeatProducers { get; } = new List<IHeatProducer>();
        public List<ThermalRadiator> Radiators { get; } = new List<ThermalRadiator>();
        
        
        public void Update(GameTime gameTime, float ambientTemperature, float environmentalThermalConductivity)
        {
            
        }
    }
}
