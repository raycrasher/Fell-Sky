using Artemis;
using FellSky.Components;
using FellSky.Game.Ships.Parts;
using FellSky.Systems.SceneGraphRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Ships
{
    public class ShipModel: IPersistable
    {
        public string Id { get; set; }
        public List<ShipPart> Parts { get; set; } = new List<ShipPart>();
        public List<Hardpoint> Hardpoints { get; set; } = new List<Hardpoint>();

        public void CreateChildEntities(EntityWorld world, Entity parentEntity)
        {
            foreach (var part in Parts)
            {
                part.CreateEntity(world, parentEntity);
            }
        }

        public Entity CreateStandAloneEntity(EntityWorld world)
        {
            var entity = world.CreateEntity();
            entity.AddComponent(new Transform());
            entity.AddComponent(new SceneGraphComponent());
            entity.AddComponent(new SceneGraphRenderRoot<StandardShipModelRenderer>());
            CreateChildEntities(world, entity);
            return entity;
        }
    }
}
