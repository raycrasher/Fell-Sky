using Artemis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using FellSky.EntityComponents;
using Microsoft.Xna.Framework;

namespace FellSky.EntityTemplates
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
