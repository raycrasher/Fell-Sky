using Artemis;
using FellSky.Game.Ships.Parts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public class DummyPartComponent: ShipPartComponent<DummyPart>, IShipPartComponent
    {
        public DummyPartComponent(DummyPart part, Entity ship)
            : base(part, ship)
        { }
    }
}
