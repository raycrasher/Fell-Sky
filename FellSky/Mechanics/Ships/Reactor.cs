using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FellSky.Graphics;

namespace FellSky.Mechanics.Ships
{
    public abstract class ReactorBase : IModule, IPowerConsumer, IHeatProducer
    {
        public ModuleClass Class { get; set; } = ModuleClass.Power;
        public string Description { get; set; } = "A reactor provides power through the generation of heat and/or charged particles.";
        public Sprite IconSprite { get; set; }
        public string IconSpriteId { get; set; }
        public string Name { get; set; } = "Reactor";
        public PowerStatus PowerStatus { get; set; }
        public float RequestedPower { get; protected set; }
        public ModuleSize Size { get; set; } = ModuleSize.Medium;
        public float Temperature { get; set; } = 300;
        public float WasteHeatOutput { get; private set; }

        public float FuelPerUnitPower { get; set; }

        // all three values below are in megawatts, actual provided is multiplied to CurrentPowerLevel.
        public float CurrentChargedParticles { get; set; } 
        public float CurrentThermalPower { get; set; }

        public float ChargedParticleProduction { get; set; }
        public float WasteHeatProduction { get; set; }
        public float ThermalProduction { get; set; }

        public float CurrentPowerLevel { get; set; } // 0 to 1;

        public float ConsumePower(float power)
        {
            return PowerStatus == PowerStatus.On ? power : 0;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (PowerStatus == PowerStatus.On)
            {
                var time = (float)gameTime.ElapsedGameTime.TotalSeconds;
                WasteHeatOutput = CurrentPowerLevel * ThermalProduction * time;
                CurrentThermalPower = CurrentPowerLevel * ThermalProduction * time;
                CurrentChargedParticles = CurrentPowerLevel * ChargedParticleProduction * time;
            }
            else
            {
                WasteHeatOutput = 0;
                CurrentThermalPower = 0;
                CurrentChargedParticles = 0;
            }
        }
    }

    public class FusionReactor: ReactorBase
    {
        
    }
}
