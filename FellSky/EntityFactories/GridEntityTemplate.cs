using Artemis.Interface;
using Artemis;
using Microsoft.Xna.Framework;
using FellSky.Components;

namespace FellSky.EntityFactories
{
    [Artemis.Attributes.ArtemisEntityTemplate("Grid")]
    public class GridEntityTemplate : IEntityTemplate
    {
        public Entity BuildEntity(Entity entity, EntityWorld entityWorld, params object[] args)
        {
            var grid = new GridComponent();

            if (args.Length > 0)
            {
                grid.GridSize = (Vector2)args[0];
                grid.GridColor = (Color)args[1];
            }

            entity.AddComponent(grid);
            return entity;
        }
    }
}
