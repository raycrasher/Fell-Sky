using Artemis;
using FellSky.Framework;
using FellSky.Components;
using FellSky.Game.Space;
using FellSky.Services;
using Microsoft.Xna.Framework;
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
        private Transform _xform = new Transform();

        private Model _sphere;
        private Camera _camera;
        private Matrix _projectionMatrix;
        

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
            _projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(20.0f), GameEngine.Instance.GraphicsDevice.Viewport.AspectRatio,
                1.0f, 10000.0f);

            _sphere = _content.Load<Model>("Meshes/uvsphere");

            var sunEffect = (BasicEffect)_sphere.Meshes[0].Effects[0];

            sunEffect.World = Matrix.Identity;
            //sunEffect.View = Matrix.CreateLookAt(new Vector3(_camera.Transform.Position, 0.5f), new Vector3(_camera.Transform.Position, 0), Vector3.UnitY);
            sunEffect.Projection = _projectionMatrix;

            sunEffect.LightingEnabled = true;
            sunEffect.Texture = _content.Load<Texture2D>("Textures/spaceobjects/sun");
            sunEffect.TextureEnabled = true;
            sunEffect.AmbientLightColor = Vector3.Zero;
            sunEffect.EmissiveColor = Vector3.One;
            sunEffect.SpecularPower = 0;
            sunEffect.DiffuseColor = Vector3.Zero;
        }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            _device.SamplerStates[0] = SamplerState.AnisotropicWrap;
            _camera = EntityWorld.GetActiveCamera();
            //_spriteBatch.Begin(transformMatrix: camera.GetViewMatrix(1.0f), samplerState: SamplerState.AnisotropicClamp);
            foreach(var entity in entities.Values)
            {
                var spaceObjectComponent = entity.GetComponent<SpaceObjectComponent>();
                if (spaceObjectComponent.Object is Planet)
                    DrawPlanet(entity, (Planet)spaceObjectComponent.Object);
                else if (spaceObjectComponent.Object is Star)
                    DrawStar(entity, (Star)spaceObjectComponent.Object);
            }
            //_spriteBatch.End();
        }

        private void DrawStar(Entity entity, Star star)
        {
            /*
            var sprite = entity.GetComponent<SpriteComponent>();
            var xform = entity.GetComponent<Transform>();
            sprite.Draw(_spriteBatch, xform);
            */

            var xform = entity.GetComponent<Transform>();

            var sunEffect = (BasicEffect)_sphere.Meshes[0].Effects[0];

            var inverse = new Vector2(1, -1) * 0.01f;

            sunEffect.World = Matrix.CreateTranslation(new Vector3(xform.Position, 0)) * Matrix.CreateRotationX(MathHelper.ToRadians(80)) * Matrix.CreateScale(1f);
            sunEffect.View = Matrix.CreateLookAt(new Vector3(_camera.Transform.Position * inverse, 20 + _camera.Zoom ), new Vector3(_camera.Transform.Position * inverse, 0), Vector3.UnitY);
                        
            _sphere.Meshes[0].Draw();
            
        }

        private void DrawPlanet(Entity entity, Planet planet)
        {

        }

        /*
        private void DrawPlanet(Entity entity, Planet planet)
        {
            var sprite = entity.GetComponent<SpriteComponent>();
            Matrix matrix;
            entity.GetWorldMatrix(out matrix);
            _xform.CopyValuesFrom(matrix);
            sprite.Draw(_spriteBatch, _xform);
            _planetShadowMask.Draw(_spriteBatch, _xform);
        }
        */
    }
}
