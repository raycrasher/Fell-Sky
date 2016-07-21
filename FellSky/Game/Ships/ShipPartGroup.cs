using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using FellSky.Game.Ships.Parts;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace FellSky.Game.Ships
{
    [Archetype]
    public class ShipPartGroup : IShipPartCollection 
    {
        public string Id { get; set; }
        public IList<ShipPart> Parts { get; set; } = new List<ShipPart>();
        public Color BaseDecalColor { get; set; }
        public Color TrimDecalColor { get; set; }

        public void CreateEntities(EntityWorld world, Entity parentEntity)
        {
            foreach(var part in Parts)
            {
                part.CreateEntity(world, parentEntity);
            }
        }
    }
}
