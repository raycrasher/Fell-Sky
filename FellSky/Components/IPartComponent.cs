using FellSky.Models.Ships.Parts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public interface IPartComponent
    {
        ShipPart Part { get; }
    }

    public interface IPartComponent<T>
        where T :ShipPart

    {
        T Part { get; set; }
    }
}
