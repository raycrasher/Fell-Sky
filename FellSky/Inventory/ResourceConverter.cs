using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Inventory
{
    public struct ResourceConverterInput
    {
        public Resource Resource;
        public float Amount;
        public float ProcessingSpeedPercentBonus;
        public float OutputAmountFlatBonus;
    }

    public struct ResourceConverterOutput
    {
        public Resource Resource;
        public float Amount;
    }

    public class ResourceConverter
    {
        public List<ResourceConverterInput> Inputs { get; } = new List<ResourceConverterInput>();
        public List<ResourceConverterOutput> Outputs { get; } = new List<ResourceConverterOutput>();
    }
}
