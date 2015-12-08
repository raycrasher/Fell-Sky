using FellSky.Ships.Parts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Ships
{



    public class PowerSubsystem
    {
        public float TotalPowerOutput { get; set; }
        public float CurrentTotalPowerRequested { get; set; }
        public List<IPowerGenerator> Generators { get; } = new List<IPowerGenerator>();
        public List<IPowerConsumer> Consumers { get; } = new List<IPowerConsumer>();

        public void Update(GameTime time)
        {
            TotalPowerOutput = Generators.Sum(g => g.PowerOutput);
            CurrentTotalPowerRequested = Consumers.Sum(c => c.RequestedPower);
        }

        public void AddConsumer(IPowerConsumer module)
        {
            Consumers.Add(module);
            Consumers.Sort((a,b)=> ((int)b.Class - (int)a.Class));
        }
    }

    public interface IPowerGenerator: IModule
    {
        float MaxLoad { get; }
        float PowerOutput { get; }
        float RequestPower(float power);
    }

    public interface IPowerConsumer: IModule
    {
        float RequestedPower { get; }
        PowerStatus PowerStatus { get; set; }
        float ConsumePower(float power);
    }
}
