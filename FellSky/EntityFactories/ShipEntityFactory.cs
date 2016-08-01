using Artemis;
using FellSky.Game.Ships.Parts;
using FellSky.Components;
using FellSky.Services;
using FellSky.Game.Ships;
using Microsoft.Xna.Framework;
using FellSky.Systems;
using System;
using Artemis.Interface;
using System.Linq;
using System.Collections.Generic;
using FellSky.Systems.SceneGraphRenderers;

namespace FellSky.EntityFactories
{
    public static class ShipEntityFactory
    {
        public static readonly Dictionary<string, Ship> Ships = new Dictionary<string, Ship>();
        public static readonly Dictionary<string, ShipPartGroup> PartGroups = new Dictionary<string, ShipPartGroup>();

        public static Entity CreateShip(this EntityWorld world, string id, Vector2 position, float rotation = 0, Vector2? scale = null, bool physics = true)
        {
            Ship ship;
            if (!Ships.TryGetValue(id, out ship))
            {
                ship = Persistence.LoadFromFile<Ship>($"Ships/{id}.json");
                Ships[id] = ship;
            }
            var shipEntity = ship.CreateEntity(world, position, rotation, scale ?? Vector2.One, physics);
            shipEntity.Refresh();
            return shipEntity;
        }

        public static ShipPartGroup CreatePartGroup(string id)
        {
            ShipPartGroup group;
            if (!PartGroups.TryGetValue(id, out group))
            {
                var path = $"Weapons/{id}.json";
                if (!System.IO.File.Exists(path))
                    throw new ArgumentException($"Cannot find PartGroup file: {path}");
                group = Persistence.LoadFromFile<ShipPartGroup>(path);

                PartGroups[id] = group;
            }
            return group;
        }
    }
}
