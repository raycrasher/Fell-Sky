using Artemis;
using FellSky.Components;
using FellSky.Framework;
using FellSky.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Systems
{
    public class BeamRendererSystem: Artemis.System.EntitySystem
    {
        //private SpriteBatch _batch;
        private GraphicsDevice _device;
        private Transform _originXform = new Transform();
        private BasicEffect _beamEffect;

        private Vertex2[] _vertices = new Vertex2[1000];
        private int[] _indices = new int[2000];
        RasterizerState _rasterizerState = new RasterizerState { CullMode = CullMode.None, ScissorTestEnable = false };

        public BeamRendererSystem()
            : base(Aspect.All(typeof(Transform), typeof(BeamComponent)))
        {
            _device = ServiceLocator.Instance.GetService<GraphicsDevice>();
            //_batch = ServiceLocator.Instance.GetService<SpriteBatch>();
            _beamEffect = new BasicEffect(_device);
            _beamEffect.VertexColorEnabled = true;
            var w = _device.Viewport.Width;
            var h = _device.Viewport.Height;
            _beamEffect.Projection = Matrix.CreateOrthographic(w, -h, -1, 1);
            _beamEffect.View = Matrix.CreateLookAt(new Vector3(w / 2, h / 2, 1), new Vector3(w / 2, h / 2, 0), Vector3.Up);

            //_beamEffect.EmissiveColor = Color.White.ToVector3();
            //_beamEffect.DiffuseColor = Color.White.ToVector3();
        }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            var camera = EntityWorld.GetActiveCamera();

            //_batch.Begin(transformMatrix: camera.GetViewMatrix(1.0f));

            //_beamEffect.Projection = camera.ProjectionMatrix;
            //_beamEffect.View = Matrix.CreateScale(1f / Constants.PhysicsUnitScale) * camera.GetViewMatrix(1.0f);

            var rState = _device.RasterizerState;
            _device.RasterizerState = _rasterizerState;

            _device.BlendState = BlendState.Additive;
            int iVertex = 0;
            int iIndex = 0;
            Texture2D lastTexture = null;

            foreach (var item in entities.Values)
            {
                var beam = item.GetComponent<BeamComponent>();
                var sprite = item.GetComponent<SpriteComponent>();

                // draw and reset if we changed textures
                if(lastTexture!=sprite.Texture)
                {
                    if(lastTexture != null)
                    {
                        _beamEffect.TextureEnabled = true;
                        _beamEffect.Texture = sprite.Texture;
                        if (iIndex > 0)
                        {
                            foreach (var technique in _beamEffect.Techniques)
                            {
                                foreach (var pass in _beamEffect.CurrentTechnique.Passes)
                                {
                                    pass.Apply();
                                    _device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _vertices, 0, iVertex, _indices, 0, iIndex / 3);
                                }
                            }
                        }
                    } else
                    {
                        _beamEffect.TextureEnabled = false;
                        _beamEffect.Texture = null;
                    }
                    
                    lastTexture = sprite.Texture;
                    iVertex = 0;
                    iIndex = 0;
                }

                _beamEffect.World = camera.GetViewMatrix(1.0f);


                Matrix matrix;
                beam.Origin.GetWorldMatrix(out matrix);
                _originXform.CopyValuesFrom(ref matrix);
                _originXform.Origin = new Vector2(0, (sprite.TextureRect.Height * beam.Scale.Y) / 2);
                int numSections = (int) Math.Ceiling(beam.Range / sprite.TextureRect.Width * beam.Scale.X);
                var direction = Utilities.CreateVector2FromAngle(_originXform.Rotation);
                var left = direction.GetPerpendicularLeft() * beam.Scale.Y * sprite.TextureRect.Height/2;
                var right = direction.GetPerpendicularRight() * beam.Scale.Y * sprite.TextureRect.Height/2;
                var offset = direction * beam.Scale.X * sprite.TextureRect.Width;

                var size = new Vector2(sprite.Texture.Width, sprite.Texture.Height);
                //var size = Vector2.One;

                Vector2 texel0 = new Vector2(sprite.TextureRect.Left, sprite.TextureRect.Top)  / size;
                Vector2 texel1 = new Vector2(sprite.TextureRect.Left, sprite.TextureRect.Bottom) / size;
                Vector2 texel2 = new Vector2(sprite.TextureRect.Right, sprite.TextureRect.Top) / size;
                Vector2 texel3 = new Vector2(sprite.TextureRect.Right, sprite.TextureRect.Bottom) / size;

                Vertex2 vtx;
                vtx.Color = beam.Color * beam.Intensity;
                //vtx.Color = beam.Color * 1;

                Vector2 posOffset;

                //if (i * 6 >= _indices.Length || i * 4 > _vertices.Length)
                //{
                //    Array.Resize(ref _indices, _indices.Length + 1000);
                //    Array.Resize(ref _vertices, _vertices.Length + 2000);
                //}


                for (int i = 0; i < numSections; i++)
                {

                    vtx.Position = _originXform.Position + left;
                    vtx.TextureCoords = texel0;
                    _vertices[iVertex++] = vtx;

                    vtx.Position = _originXform.Position + right;
                    vtx.TextureCoords = texel1;
                    _vertices[iVertex++] = vtx;

                    posOffset = _originXform.Position + offset;                    
                    
                    vtx.Position = posOffset + left;
                    vtx.TextureCoords = texel2;
                    _vertices[iVertex++] = vtx;

                    vtx.Position = posOffset + right;
                    vtx.TextureCoords = texel3;
                    _vertices[iVertex++] = vtx;

                    _indices[iIndex++] = iVertex - 4;
                    _indices[iIndex++] = iVertex - 3;
                    _indices[iIndex++] = iVertex - 2;
                    _indices[iIndex++] = iVertex - 3;
                    _indices[iIndex++] = iVertex - 1;
                    _indices[iIndex++] = iVertex - 2;

                    //sprite.Draw(_batch, _originXform, new Color(0,0,255,128));
                    _originXform.Position = posOffset;
                }

                // draw fadeout
                posOffset = _originXform.Position + offset / 4;
                vtx.Color = beam.Color * 0;

                vtx.Position = posOffset + left;
                vtx.TextureCoords = texel2;
                _vertices[iVertex++] = vtx;

                vtx.Position = posOffset + right;
                vtx.TextureCoords = texel3;
                _vertices[iVertex++] = vtx;

                _indices[iIndex++] = iVertex - 4;
                _indices[iIndex++] = iVertex - 3;
                _indices[iIndex++] = iVertex - 2;
                _indices[iIndex++] = iVertex - 3;
                _indices[iIndex++] = iVertex - 1;
                _indices[iIndex++] = iVertex - 2;
            }

            // draw geometry
            if (iVertex > 0 && iIndex > 0 && lastTexture != null)
            {
                _beamEffect.TextureEnabled = true;
                _beamEffect.Texture = lastTexture;

                foreach (var pass in _beamEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    _device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _vertices, 0, iVertex, _indices, 0, iIndex / 3);
                    //_device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _vertices, 0, iVertex, _indices, 0, iIndex / 3);
                }
            }
            _device.RasterizerState = rState;
        }
        
    }
}
