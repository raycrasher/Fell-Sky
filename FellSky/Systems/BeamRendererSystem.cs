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

        private Vertex[] _vertices = new Vertex[1000];
        private int[] _indices = new int[2000];

        public BeamRendererSystem()
            : base(Aspect.All(typeof(Transform), typeof(BeamComponent)))
        {
            _device = ServiceLocator.Instance.GetService<GraphicsDevice>();
            //_batch = ServiceLocator.Instance.GetService<SpriteBatch>();
            _beamEffect = new BasicEffect(_device);
            _beamEffect.VertexColorEnabled = true;
        }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            var camera = EntityWorld.GetActiveCamera();

            //_batch.Begin(transformMatrix: camera.GetViewMatrix(1.0f));

            _beamEffect.Projection = camera.ProjectionMatrix;
            _beamEffect.View = Matrix.CreateScale(1f / Constants.PhysicsUnitScale) * camera.GetViewMatrix(1.0f);

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
                        if (iIndex > 0)
                        {
                            foreach (var technique in _beamEffect.Techniques)
                            {
                                foreach(var pass in technique.Passes)
                                {
                                    pass.Apply();
                                    _device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _vertices, 0, iVertex, _indices, 0, iIndex / 3);
                                }
                            }
                        }
                    }
                    lastTexture = sprite.Texture;
                    iVertex = 0;
                    iIndex = 0;
                }


                Matrix matrix;
                beam.Origin.GetWorldMatrix(out matrix);
                _originXform.CopyValuesFrom(ref matrix);
                _originXform.Origin = new Vector2(0, (sprite.TextureRect.Height * beam.Scale.Y) / 2);
                int numSections = (int) Math.Ceiling(beam.Range / sprite.TextureRect.Width * beam.Scale.X);
                var direction = Utilities.CreateVector2FromAngle(_originXform.Rotation);
                var left = direction.GetPerpendicularLeft();
                var right = direction.GetPerpendicularRight();
                var offset = direction * beam.Scale.X;

                Vector2 texel0 = Vector2.Zero;
                Vector2 texel1 = new Vector2(0, sprite.TextureRect.Height);
                Vector2 texel2 = new Vector2(sprite.TextureRect.Width, 0);
                Vector2 texel3 = new Vector2(sprite.TextureRect.Width, sprite.TextureRect.Height);

                Vertex vtx;
                vtx.Color = beam.Color;

                vtx.Position = _originXform.Position + left;
                vtx.TextureCoords = texel0;
                _vertices[iVertex++] = vtx;

                vtx.Position = _originXform.Position + right;
                vtx.TextureCoords = texel1;
                _vertices[iVertex++] = vtx;

                for (int i = 0; i < numSections; i++)
                {
                    if(i * 6 >= _indices.Length || i * 4 > _vertices.Length)
                    {
                        Array.Resize(ref _indices, _indices.Length + 1000);
                        Array.Resize(ref _vertices, _vertices.Length + 2000);
                    }
                    

                    var posOffset = _originXform.Position + offset;                    
                    
                    vtx.Position = posOffset + left;
                    vtx.TextureCoords = texel2;
                    _vertices[iVertex++] = vtx;

                    vtx.Position = posOffset + left;
                    vtx.TextureCoords = texel3;
                    _vertices[iVertex++] = vtx;

                    _indices[iIndex++] = iVertex - 4;
                    _indices[iIndex++] = iVertex - 3;
                    _indices[iIndex++] = iVertex - 2;
                    _indices[iIndex++] = iVertex - 2;
                    _indices[iIndex++] = iVertex - 3;
                    _indices[iIndex++] = iVertex - 1;

                    //sprite.Draw(_batch, _originXform, new Color(0,0,255,128));
                    _originXform.Position = posOffset;
                }
            }

            // draw geometry
            if (iVertex > 0 && iIndex > 0 && lastTexture != null)
            {
                foreach (var technique in _beamEffect.Techniques)
                {
                    foreach (var pass in technique.Passes)
                    {
                        pass.Apply();
                        _device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _vertices, 0, iVertex, _indices, 0, iIndex / 3);
                    }
                }
            }
        }
        
    }
}
