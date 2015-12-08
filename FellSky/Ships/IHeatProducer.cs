using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Ships
{
    public interface IHeatProducer
    {
        float Temperature { get; }
        float WasteHeatOutput { get; }
    }
}
