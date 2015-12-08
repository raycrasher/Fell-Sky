using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Inventory
{
    public interface IResourceConsumer
    {
        Resource Resource { get; set; }
        float AmountRequired { get; }
        float AmountAvailable { get; set; }
        float AmountConsumed { get; }
    }
}
