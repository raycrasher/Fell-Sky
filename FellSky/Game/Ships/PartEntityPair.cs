using Artemis;
using FellSky.Game.Ships.Parts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Ships
{
    public class PartEntityPair
    {
        public PartEntityPair() { }
        public PartEntityPair(ShipPart part, Entity entity)
        {
            Part = part;
            Entity = entity;
        }

        public ShipPart Part;
        public Entity Entity;
    }
}
