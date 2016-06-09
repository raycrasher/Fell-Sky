using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;

namespace FellSky.Game.Ships.Parts
{
    public class DummyPart : ShipPart
    {
        public override Entity CreateEntity(EntityWorld world, Entity ship, int? index = default(int?))
        {
            // we don't create an entity, so return null.
            return null;
        }
    }
}
