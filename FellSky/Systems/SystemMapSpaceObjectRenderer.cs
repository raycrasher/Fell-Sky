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

        private Model _sphereModel;
        private Model _haloModel;

        private Camera _camera;
        private Matrix _projectionMatrix;

        private BasicEffect _sunEffect, _planetEffect, _haloEffect, _starGlowEffect;
        private Vertex3[] _quad = new Vertex3[] {
            new Vertex3 { Position = new Vector3(-1,-1,0), Color = Color.White, TextureCoords=new Vector2(0,0) },
            new Vertex3 { Position = new Vector3(-1, 1,0), Color = Color.White, TextureCoords=new Vector2(0,1) },
            new Vertex3 { Position = new Vector3( 1,-1,0), Color = Color.White, TextureCoords=new Vector2(1,0) },
            new Vertex3 { Position = new Vector3( 1, 1,0), Color = Color.White, TextureCoords=new Vector2(1,1) },
        };

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
                MathHelper.ToRadians(45.0f), GameEngine.Instance.GraphicsDevice.Viewport.AspectRatio,
                1.0f, 10000.0f);
            _projectionMatrix = Matrix.CreateOrthographic(_device.Viewport.Width / 100f, _device.Viewport.Height / 100f, 1.0f, 10000.0f);

            _sphereModel = _content.Load<Model>("Meshes/uvsphere");


            _sunEffect = new BasicEffect(_device);
            //var sunEffect = (BasicEffect)_sphereModel.Meshes[0].Effects[0];

            _sunEffect.World = Matrix.Identity;
            _sunEffect.Projection = _projectionMatrix;
            _sunEffect.Texture = _content.Load<Texture2D>("Textures/spaceobjects/sun-white");
            _sunEffect.TextureEnabled = true;
            _sunEffect.AmbientLightColor = Vector3.Zero;
            _sunEffect.EmissiveColor = Vector3.One;
            _sunEffect.SpecularPower = 0;
            _sunEffect.DiffuseColor = Vector3.Zero;
            _sunEffect.PreferPerPixelLighting = true;

            _planetEffect = new BasicEffect(_device);
            _planetEffect.Texture = _content.Load<Texture2D>("Textures/spaceobjects/rock1");
            _planetEffect.LightingEnabled = true;
            _planetEffect.TextureEnabled = true;
            _planetEffect.AmbientLightColor = Vector3.One;
            _planetEffect.EmissiveColor = Vector3.Zero;
            _planetEffect.SpecularPower = 0.5f;
            _planetEffect.DiffuseColor = Vector3.One;
            _planetEffect.PreferPerPixelLighting = true;
            _planetEffect.Projection = _projectionMatrix;


            _haloModel = _content.Load<Model>("Meshes/halo");
            _haloEffect = (BasicEffect)_haloModel.Meshes[0].Effects[0];
            _haloEffect.Texture = _content.Load<Texture2D>("Textures/spaceobjects/halo");
            _haloEffect.TextureEnabled = true;
            _haloEffect.PreferPerPixelLighting = true;
            _haloEffect.SpecularPower = 0;
            _haloEffect.AmbientLightColor = Vector3.Zero;
            _haloEffect.EmissiveColor = Vector3.One;
            _haloEffect.DiffuseColor = Vector3.Zero;
            _haloEffect.Projection = _projectionMatrix;

            _starGlowEffect = new BasicEffect(_device);
            _starGlowEffect.Texture = _content.Load<Texture2D>("Textures/spaceobjects/starglow");
            _starGlowEffect.TextureEnabled = true;
            _starGlowEffect.PreferPerPixelLighting = true;
            _starGlowEffect.SpecularPower = 0;
            _starGlowEffect.AmbientLightColor = Vector3.Zero;
            _starGlowEffect.EmissiveColor = Vector3.One;
            _starGlowEffect.DiffuseColor = Vector3.Zero;
            _starGlowEffect.Projection = _projectionMatrix;
        }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            _device.SamplerStates[0] = SamplerState.AnisotropicWrap;
            _camera = EntityWorld.GetActiveCamera();
            foreach(var entity in entities.Values)
            {
                var spaceObjectComponent = entity.GetComponent<SpaceObjectComponent>();
                if (spaceObjectComponent.Object is Planet)
                    DrawPlanet(entity, (Planet)spaceObjectComponent.Object);
                else if (spaceObjectComponent.Object is Star)
                    DrawStar(entity, (Star)spaceObjectComponent.Object);
            }
        }

        private void DrawStar(Entity entity, Star star)
        {
            var xform = entity.GetComponent<Transform>();
            var inverse = new Vector2(1, -1) * 0.01f;
            
            _starGlowEffect.World = Matrix.CreateTranslation(new Vector3(xform.Position, 0)) * Matrix.CreateScale(new Vector3(5, 5, 1));
            _starGlowEffect.View = Matrix.CreateLookAt(new Vector3(_camera.Transform.Position * inverse, 20 + _camera.Zoom), new Vector3(_camera.Transform.Position * inverse, 0), Vector3.UnitY);

            // draw glow
            foreach (var pass in _starGlowEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _device.DrawUserPrimitives(PrimitiveType.TriangleStrip, _quad, 0, 2);
            }

            _sphereModel.Meshes[0].MeshParts[0].Effect = _sunEffect;
            _sunEffect.World = Matrix.CreateTranslation(new Vector3(xform.Position, 0)) * Matrix.CreateRotationX(MathHelper.ToRadians(80)) * Matrix.CreateScale(1f);
            _sunEffect.View = _starGlowEffect.View;
                        
            _sphereModel.Meshes[0].Draw();
            DrawHalo(entity);
        }

        private void DrawHalo(Entity entity)
        {
            var stencilState = _device.DepthStencilState;
            _device.DepthStencilState = DepthStencilState.None;

            var xform = entity.GetComponent<Transform>();
            var inverse = new Vector2(1, -1) * 0.01f;

            _haloEffect.World = Matrix.CreateTranslation(new Vector3(xform.Position, 0)) * Matrix.CreateScale(1f);
            _haloEffect.View = Matrix.CreateLookAt(new Vector3(_camera.Transform.Position * inverse, 20 + _camera.Zoom), new Vector3(_camera.Transform.Position * inverse, 0), Vector3.UnitY);

            _haloModel.Meshes[0].Draw();

            _device.DepthStencilState = stencilState;
        }

        private void DrawPlanet(Entity entity, Planet planet)
        {
            var xform = entity.GetComponent<Transform>();
            _sphereModel.Meshes[0].MeshParts[0].Effect = _planetEffect;
            var inverse = new Vector2(1, -1) * 0.01f;

            _planetEffect.World = Matrix.CreateTranslation(new Vector3(xform.Position, 0)) * Matrix.CreateRotationX(MathHelper.ToRadians(80)) * Matrix.CreateScale(1f);
            _planetEffect.View = Matrix.CreateLookAt(new Vector3(_camera.Transform.Position * inverse, 20 + _camera.Zoom), new Vector3(_camera.Transform.Position * inverse, 0), Vector3.UnitY);

            //_planetEffect.Texture = entity.GetComponent<SpaceObjectComponent>().Texture;

            _sphereModel.Meshes[0].Draw();
            DrawHalo(entity);
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
