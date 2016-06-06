using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using Microsoft.Xna.Framework;
using FellSky.Game.Ships;
using FellSky.Components;
using FellSky.Game.Ships.Parts;

namespace FellSky.EntityFactories
{
    class ShipEntityTemplate : Artemis.Interface.IEntityTemplate
    {
        Dictionary<string, Ship> _ships = new Dictionary<string, Ship>();
        public Entity BuildEntity(Entity shipEntity, EntityWorld world, params object[] args)
        {
            var id = (string) args[0];
            var position = (Vector2)(args[1] ?? Vector2.Zero);
            var rotation = (float)(args[2] ?? 0);

            Ship ship;
            if (!_ships.TryGetValue(id, out ship))
            {
                ship = Persistence.LoadFromFile<Ship>($"Ships/{id}.json");
                _ships[id] = ship;
            }

            shipEntity.AddComponent(new ShipComponent(ship));
            shipEntity.AddComponent(new SceneGraphComponent());
            foreach(var part in ship.Parts)
            {
                var partEntity = part.CreateEntity(world, shipEntity);
            }

            return shipEntity;
        }
        
    }
}
