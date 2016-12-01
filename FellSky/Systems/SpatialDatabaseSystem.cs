using Artemis;
using Artemis.Interface;
using FellSky.Components;
using Microsoft.Xna.Framework;
using SharpSteer2.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Systems
{
    class SpatialDatabaseSystem: Artemis.System.EntitySystem
    {
        public LocalityQueryProximityDatabase<Entity> Ships { get; private set; }

        public Vector2 Dimensions { get; private set; }
        public float CellSize { get; private set; }


        public SpatialDatabaseSystem()
            : base(Aspect.One(typeof(ShipComponent)))
        { }

        public SpatialDatabaseSystem(Vector2? dimensions = null, float cellSize = 1000)
        {
            Dimensions = dimensions ?? new Vector2(1e6f, 1e6f);
            CellSize = cellSize;
            Ships = new LocalityQueryProximityDatabase<Entity>(
                System.Numerics.Vector3.Zero, 
                new System.Numerics.Vector3(Dimensions.X, Dimensions.Y, 1), 
                new System.Numerics.Vector3(Dimensions.X / CellSize, Dimensions.Y / CellSize, 1));
        }

        public override void LoadContent()
        {
            base.LoadContent();
            EntityWorld.EntityManager.RemovedEntityEvent += OnRemoveEntity;
        }

        private void OnRemoveEntity(Entity entity)
        {
            if (entity.HasComponent<ShipComponent>())
            {
                var token = entity.GetComponent<SpatialTokenComponent>().Token;
                token.Dispose();
            }
        }

        public override void OnAdded(Entity entity)
        {
            if (entity.HasComponent<ShipComponent>())
            {
                entity.AddComponent(new SpatialTokenComponent { Token = Ships.AllocateToken(entity) });
            }
        }
    }
}
