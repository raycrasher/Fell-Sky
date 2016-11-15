using Artemis.Interface;
using Artemis;
using Microsoft.Xna.Framework;
using FellSky.Components;

namespace FellSky
{
    public static class GridEntityFactory
    {
        public static Entity CreateGrid(this EntityWorld world, Vector2 gridSize, Color color)
        {
            var entity = world.CreateEntity();
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
