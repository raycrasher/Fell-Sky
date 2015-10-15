using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Mechanics
{
    public interface IResourceProducer
    {
        Resource Resource { get; set; }
        float MaxProduction { get; }
    }
}
