using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Inventory
{
    public interface IResourceStorage
    {
        Resource Resource { get; set; }
        float StoredAmount { get; }
        float MaxStorage { get; }
    }
}
