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
        private SpriteComponent _planetShadowMask;

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
            _planetShadowMask = ServiceLocator.Instance.GetService<ISpriteManagerService>().CreateSpriteComponent("planetshadowmask");
        }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            var camera = EntityWorld.GetActiveCamera();
            _spriteBatch.Begin(transformMatrix: camera.GetViewMatrix(1.0f), samplerState: SamplerState.AnisotropicClamp);
            foreach(var entity in entities.Values)
            {
                var spaceObjectComponent = entity.GetComponent<SpaceObjectComponent>();
                if (spaceObjectComponent.Object is Planet)
                    DrawPlanet(entity, (Planet)spaceObjectComponent.Object);
                else if (spaceObjectComponent.Object is Star)
                    DrawStar(entity, (Star)spaceObjectComponent.Object);
            }
            _spriteBatch.End();
        }

        private void DrawStar(Entity entity, Star star)
        {
            var sprite = entity.GetComponent<SpriteComponent>();
            var xform = entity.GetComponent<Transform>();
            sprite.Draw(_spriteBatch, xform);
        }

        private void DrawPlanet(Entity entity, Planet planet)
        {
            var sprite = entity.GetComponent<SpriteComponent>();
            var xform = entity.GetComponent<Transform>();
            sprite.Draw(_spriteBatch, xform);
            _planetShadowMask.Draw(_spriteBatch, xform);
        }
    }
}
