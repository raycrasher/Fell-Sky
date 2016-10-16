using FellSky.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Framework
{
    public class FastSpriteBatch
    {
        private BasicEffect _defaultEffect;
        private Vertex3CTN[] _vertices;
        private int[] _indices;
        private int _vertexCount, _indexCount;
        private Vertex3CTN[] _quadVertices = new Vertex3CTN[4];
        private int[] _quadIndices = new int[]{ 0, 1, 2, 1, 3, 2 };
        private Effect _effect;
        private GraphicsDevice _device;
        private Texture2D _texture;

        public GraphicsDevice GraphicsDevice => _device;
        public int NumVertices => _vertexCount;
        public int NumIndices => _indexCount;

        public FastSpriteBatch(GraphicsDevice device, int initialVertexCount = 10000)
        {
            _device = device;
            _vertices = new Vertex3CTN[initialVertexCount];
            _indices = new int[_vertices.Length * 3 / 2];
            _defaultEffect = new BasicEffect(device);
            _defaultEffect.VertexColorEnabled = true;
            _defaultEffect.TextureEnabled = true;
        }

        public void Begin(Effect effect)
        {
            _effect = effect;
            _vertexCount = 0;
            _indexCount = 0;
            _texture = null;
        }

        public void Begin(Matrix world, Matrix projection, Matrix view)
        {
            _effect = _defaultEffect;
            _defaultEffect.World = world;
            _defaultEffect.Projection = projection;
            _defaultEffect.View = view;
            _vertexCount = 0;
            _indexCount = 0;
            _texture = null;
        }

        public void Draw(Texture2D texture, ref Matrix transform, FloatRect? sourceRectangle = null, Color? color=null, Vector3? normal = null)
        {
            Vertex3CTN vtx;
            vtx.Color = color ?? Color.White;
            vtx.Normal = normal ?? Vector3.Forward;
            FloatRect texRect = sourceRectangle ?? new FloatRect(texture.Bounds);

            Vector2 texCoordLT = texture.GetUV(texRect.Left, texRect.Top);
            Vector2 texCoordBR = texture.GetUV(texRect.Right, texRect.Bottom);

            vtx.Position = Vector3.Zero;
            vtx.TextureCoords = texCoordLT;
            _quadVertices[0] = vtx;
            vtx.Position = new Vector3(0, texRect.Height, 0);
            vtx.TextureCoords = new Vector2(texCoordLT.X, texCoordBR.Y);
            _quadVertices[1] = vtx;
            vtx.Position = new Vector3(texRect.Width, 0, 0);
            vtx.TextureCoords = new Vector2(texCoordBR.X, texCoordLT.Y);
            _quadVertices[2] = vtx;
            vtx.Position = new Vector3(texRect.Width, texRect.Height, 0);
            vtx.TextureCoords = texCoordBR;
            _quadVertices[3] = vtx;

            DrawVertices(texture, _quadVertices, _quadIndices, ref transform);
        }

        public void Draw(SpriteComponent sprite, ref Matrix transform, Color? color=null, Vector3? normal=null)
        {
            Vertex3CTN vtx;
            vtx.Color = color ?? Color.White;
            vtx.Normal = normal ?? Vector3.Forward;
            
            var tex = sprite.Texture;

            Vector2 texCoordLT = tex.GetUV(sprite.TextureRect.Left, sprite.TextureRect.Top);
            Vector2 texCoordBR = tex.GetUV(sprite.TextureRect.Right, sprite.TextureRect.Bottom);

            vtx.Position = Vector3.Zero;
            vtx.TextureCoords = texCoordLT;
            _quadVertices[0] = vtx;
            vtx.Position = new Vector3(0, sprite.TextureRect.Height, 0);
            vtx.TextureCoords = new Vector2(texCoordLT.X, texCoordBR.Y);
            _quadVertices[1] = vtx;
            vtx.Position = new Vector3(sprite.TextureRect.Width, 0, 0);
            vtx.TextureCoords = new Vector2(texCoordBR.X, texCoordLT.Y);
            _quadVertices[2] = vtx;
            vtx.Position = new Vector3(sprite.TextureRect.Width, sprite.TextureRect.Height, 0);
            vtx.TextureCoords = texCoordBR;
            _quadVertices[3] = vtx;

            DrawVertices(tex, _quadVertices, _quadIndices, ref transform);
        }

        public void End()
        {
            if (_indexCount > 0)
            {
                RenderImpl(_texture);
            }
        }

        public void DrawVertices(Texture2D texture, Vertex3CTN[] vertices, int[] indices, ref Matrix transform)
        {
            if (_texture == null)
                _texture = texture;
            if (_texture != texture)
            {
                RenderImpl(texture);
                _vertexCount = 0;
                _indexCount = 0;
                _texture = texture;
            }
            EnsureSpace(indices.Length, vertices.Length);

            int baseIndex = _vertexCount;

            for (int i = 0; i < vertices.Length; i++) {
                var vtx = vertices[i];

                Vector3.Transform(ref vtx.Position, ref transform, out vtx.Position);
                if (Math.Abs(vtx.Position.Z) > float.Epsilon) throw new InvalidProgramException();
                _vertices[_vertexCount++] = vtx;
            }
            for(int i = 0; i < indices.Length; i++)
            {
                _indices[_indexCount++] = indices[i] + baseIndex;
            }
        }

        private void RenderImpl(Texture2D texture)
        {
            foreach(var pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _device.Textures[0] = texture;
                _device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _vertices, 0, _vertexCount, _indices, 0, _indexCount / 3);
            }
        }

        void EnsureSpace(int indexSpace, int vertexSpace)
        {
            if (_indexCount + indexSpace >= _indices.Length)
                Array.Resize(ref _indices, Math.Max(_indexCount + indexSpace, _indices.Length * 2));
            if (_vertexCount + vertexSpace >= _vertices.Length)
                Array.Resize(ref _vertices, Math.Max(_vertexCount + vertexSpace, _vertices.Length * 2));
        }
    }
}
