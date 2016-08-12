using Artemis;
using FellSky.Components;
using FellSky.Game.Space;
using FellSky.Services;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Systems
{
    public class SystemMapSpaceObjectRenderer: Artemis.System.EntitySystem
    {
        private ContentManager _content;
        private GraphicsDevice _device;
        private SpriteBatch _spriteBatch;

        //private Model _sphereMesh;

        public SystemMapSpaceObjectRenderer()
            : base(Aspect.All(typeof(SpaceObjectComponent)))
        {
            _device = ServiceLocator.Instance.GetService<GraphicsDevice>();
            _content = ServiceLocator.Instance.GetService<ContentManager>();
            _spriteBatch = ServiceLocator.Instance.GetService<SpriteBatch>();
        }

        public override void LoadContent()
        {
            //_sphereMesh = _content.Load<Model>("Meshes/Sphere");
        }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            foreach(var entity in entities.Values)
            {
                var spaceObjectComponent = entity.GetComponent<SpaceObjectComponent>();
                if (spaceObjectComponent.Object is Planet)
                    DrawPlanet(entity);
                else if (spaceObjectComponent.Object is Star)
                    DrawStar(entity);
            }
        }

        private void DrawStar(Entity entity)
        {
            var camera = EntityWorld.GetActiveCamera();
        }

        private void DrawPlanet(Entity entity)
        {
            throw new NotImplementedException();
        }
    }
}
