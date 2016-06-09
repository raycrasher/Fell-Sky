using FellSky.Game.Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public class ShipPartGroupComponent: IShipPartCollectionComponent
    {
        public ShipPartGroupComponent(ShipPartGroup group)
        {
            Model = group;
        }
        public IShipPartCollection Model { get; set; }
    }
}
