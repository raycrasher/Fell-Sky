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
        public float ModelScale = 200;
        public float ZPosition = 0;
        private ContentManager _content;
        private GraphicsDevice _device;
        private SpriteBatch _spriteBatch;
        private SpriteComponent _planetShadowMask;
        private Transform _xform = new Transform();

        private RasterizerState _antiAliasRasterizerState = new RasterizerState { MultiSampleAntiAlias = true };

        private Model _sphereModel;
        private Model _haloModel;

        private Camera _camera;
        private Matrix _projectionMatrix;

        private BasicEffect _sunEffect, _planetEffect, _haloEffect, _starGlowEffect;
        private Vertex3CT[] _quad = new Vertex3CT[] {
            new Vertex3CT { Position = new Vector3(-1,-1,0), Color = Color.White, TextureCoords=new Vector2(0,0) },
            new Vertex3CT { Position = new Vector3(-1, 1,0), Color = Color.White, TextureCoords=new Vector2(0,1) },
            new Vertex3CT { Position = new Vector3( 1,-1,0), Color = Color.White, TextureCoords=new Vector2(1,0) },
            new Vertex3CT { Position = new Vector3( 1, 1,0), Color = Color.White, TextureCoords=new Vector2(1,1) },
        };
        private Model _sphereModelLowPoly;

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
            _projectionMatrix = Matrix.CreateOrthographic(_device.Viewport.Width, _device.Viewport.Height, 1.0f, 10000.0f);

            _sphereModel = _content.Load<Model>("Meshes/uvsphere_high_lod");
            _sphereModelLowPoly = _content.Load<Model>("Meshes/uvsphere");


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
            _planetEffect.AmbientLightColor = new Vector3(0.1f);
            _planetEffect.EmissiveColor = Vector3.Zero;
            _planetEffect.SpecularPower = 0.1f;
            _planetEffect.DiffuseColor = Vector3.One;
            _planetEffect.Projection = _projectionMatrix;
            _planetEffect.PreferPerPixelLighting = true;
            


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
            var previousRState = _device.RasterizerState;
            _device.RasterizerState = _antiAliasRasterizerState;
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
            _device.RasterizerState = previousRState;
        }

        private void DrawStar(Entity entity, Star star)
        {
            var xform = entity.GetComponent<Transform>();
            var inverse = new Vector2(1, -1) * 0.01f;

            //_starGlowEffect.View = Matrix.CreateLookAt(new Vector3(_camera.Transform.Position * inverse, 100), new Vector3(_camera.Transform.Position * inverse, 0), Vector3.UnitY) * Matrix.CreateScale(_camera.Zoom);
            _starGlowEffect.View = _camera.GetViewMatrix(1.0f);

            _starGlowEffect.World = Matrix.CreateScale(new Vector3(50, 50, 1)) * Matrix.CreateTranslation(new Vector3(xform.Position, ZPosition));
            _starGlowEffect.Alpha = 0.1f;
            foreach (var pass in _starGlowEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _device.DrawUserPrimitives(PrimitiveType.TriangleStrip, _quad, 0, 2);
            }

            _starGlowEffect.Alpha = 1f;
            _starGlowEffect.World = Matrix.CreateScale(new Vector3(5, 5, 1)) * Matrix.CreateTranslation(new Vector3(xform.Position, ZPosition) * ModelScale);
            //_starGlowEffect.View = _camera.GetViewMatrix(1.0f);

            // draw glow
            foreach (var pass in _starGlowEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _device.DrawUserPrimitives(PrimitiveType.TriangleStrip, _quad, 0, 2);
            }

            _sphereModel.Meshes[0].MeshParts[0].Effect = _sunEffect;
            _sunEffect.World = Matrix.CreateRotationX(MathHelper.ToRadians(80)) * Matrix.CreateScale(new Vector3(xform.Scale, xform.Scale.X) * ModelScale) * Matrix.CreateTranslation(new Vector3(xform.Position, ZPosition));
            _sunEffect.View = _starGlowEffect.View;
                        
            _sphereModel.Meshes[0].Draw();
            
            DrawHalo(entity, Matrix.CreateScale(new Vector3(xform.Scale, xform.Scale.X) * ModelScale) * Matrix.CreateTranslation(new Vector3(xform.Position, ZPosition)), _sunEffect.View);
        }

        private void DrawHalo(Entity entity, Matrix worldMatrix, Matrix viewMatrix, float alpha=1f)
        {
            var rState = _device.RasterizerState;
            _device.RasterizerState = RasterizerState.CullNone;
            var stencilState = _device.DepthStencilState;
            _device.DepthStencilState = DepthStencilState.None;
            _haloEffect.Alpha = alpha;
            _haloEffect.World = worldMatrix;
            _haloEffect.View = viewMatrix;
            _haloModel.Meshes[0].Draw();

            _device.DepthStencilState = stencilState;
            _device.RasterizerState = rState;
        }

        private void DrawPlanet(Entity entity, Planet planet)
        {
            var xform = entity.GetComponent<Transform>();
            _sphereModelLowPoly.Meshes[0].MeshParts[0].Effect = _planetEffect;
            var inverse = new Vector2(1, -1) * 0.01f;

            _planetEffect.World = Matrix.CreateRotationX(MathHelper.ToRadians(80)) * Matrix.CreateScale(ModelScale) * Matrix.CreateTranslation(new Vector3(xform.Position, ZPosition));
            _planetEffect.View = _camera.GetViewMatrix(1.0f);

            _planetEffect.DirectionalLight0.Enabled = true;
            _planetEffect.DirectionalLight0.DiffuseColor = new Color(255,255,200).ToVector3() * 0.3f;
            _planetEffect.DirectionalLight0.Direction = new Vector3(xform.Position, 0);
            //_planetEffect.Texture = entity.GetComponent<SpaceObjectComponent>().Texture;

            _sphereModelLowPoly.Meshes[0].Draw();
            DrawHalo(entity, Matrix.CreateScale(ModelScale) * Matrix.CreateTranslation(new Vector3(xform.Position, ZPosition)), _planetEffect.View, 0.8f);
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
