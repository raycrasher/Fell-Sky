using Artemis.Interface;
using FellSky.Mechanics.Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.EntityComponents
{
    public class ShipComponent: IComponent
    {
        public Ship Ship { get; set; }
    }
}
