using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Inventory
{
    public interface IInventoryItem
    {
        string Id { get; }
        string Name { get; }
        string Description { get; }
    }
}
