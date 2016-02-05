using Artemis.Interface;
using Artemis;
using Microsoft.Xna.Framework;
using FellSky.Components;

namespace FellSky.EntityFactories
{
    public class GridEntityFactory
    {
        public GridEntityFactory(EntityWorld world)
        {
            World = world;
        }

        public EntityWorld World { get; private set; }

        public Entity CreateGrid(Vector2 gridSize, Color color)
        {
            var entity = World.CreateEntity();
            var grid = new GridComponent
            {
                GridSize = gridSize,
                GridColor = color
            };
            
            entity.AddComponent(grid);
            return entity;
        }
    }
}
