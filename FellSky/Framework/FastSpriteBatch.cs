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
        
        private Vertex3CTN[] _vertices;
        private int[] _indices;
        private int _vertexCount, _indexCount;
        private Vertex3CTN[] _quadVertices = new Vertex3CTN[4];
        private int[] _quadIndices = new int[] { 0, 1, 2, 1, 3, 2 };

        public Effect Effect { get; set; }
        public GraphicsDevice Device { get; set; }

        public int NumVertices => _vertexCount;
        public int NumIndices => _indexCount;

        public Matrix ViewMatrix {
            get {
                var matrices = Effect as IEffectMatrices;
                if (matrices == null)
                    throw new InvalidOperationException();
                return matrices.View;
            }
            set
            {
                var matrices = Effect as IEffectMatrices;
                if (matrices == null)
                    throw new InvalidOperationException();
                matrices.View = value;
            }
        }

        public Matrix ProjectionMatrix
        {
            get
            {
                var matrices = Effect as IEffectMatrices;
                if (matrices == null)
                    throw new InvalidOperationException();
                return matrices.Projection;
            }
            set
            {
                var matrices = Effect as IEffectMatrices;
                if (matrices == null)
                    throw new InvalidOperationException();
                matrices.Projection = value;
            }
        }

        public Matrix WorldMatrix
        {
            get
            {
                var matrices = Effect as IEffectMatrices;
                if (matrices == null)
                    throw new InvalidOperationException();
                return matrices.World;
            }
            set
            {
                var matrices = Effect as IEffectMatrices;
                if (matrices == null)
                    throw new InvalidOperationException();
                matrices.World = value;
            }
        }

        public FastSpriteBatch(GraphicsDevice device, int initialVertexCount = 10000)
        {
            _vertices = new Vertex3CTN[initialVertexCount];
            _indices = new int[_vertices.Length * 3 / 2];
            Device = device;
        }

        public void Reset(Effect effect)
        {
            Effect = effect;
            _vertexCount = 0;
            _indexCount = 0;
        }

        public void Draw(SpriteComponent sprite, ref Matrix transform, Color? color=null, Vector3? normal=null)
        {
            Vertex3CTN vtx;
            vtx.Color = color ?? Color.White;
            vtx.Normal = normal ?? Vector3.Forward;
            vtx.Position = Vector3.Zero;
            var tex = sprite.Texture;
            vtx.TextureCoords = GetUV(tex, sprite.TextureRect.Left, sprite.TextureRect.Top);
            _quadVertices[0] = vtx;
            vtx.Position = new Vector3(0, sprite.TextureRect.Height, 0);
            vtx.TextureCoords = GetUV(tex, sprite.TextureRect.Left, sprite.TextureRect.Bottom);
            _quadVertices[1] = vtx;
            vtx.Position = new Vector3(sprite.TextureRect.Width, 0, 0);
            vtx.TextureCoords = GetUV(tex, sprite.TextureRect.Right, sprite.TextureRect.Top);
            _quadVertices[2] = vtx;
            vtx.Position = new Vector3(sprite.TextureRect.Width, sprite.TextureRect.Height, 0);
            vtx.TextureCoords = GetUV(tex, sprite.TextureRect.Right, sprite.TextureRect.Bottom);
            _quadVertices[3] = vtx;

            Draw(_quadVertices, _quadIndices, ref transform);
        }

        public void Draw(Vertex3CTN[] vertices, int[] indices, ref Matrix transform)
        {
            EnsureSpace(indices.Length, vertices.Length);

            int baseIndex = _vertexCount;

            for (int i = 0; i < vertices.Length; i++) {
                var vtx = vertices[i];

                Vector3.Transform(ref vtx.Position, ref transform, out vtx.Position);
                _vertices[_vertexCount++] = vtx;
            }
            for(int i = 0; i < indices.Length; i++)
            {
                _indices[_indexCount++] = indices[i] + baseIndex;
            }
        }

        public void Render()
        {
            var effect = Effect;
            foreach(var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _vertices, 0, _vertexCount, _indices, 0, _indexCount / 3);
            }
        }

        Vector2 GetUV(Texture2D texture, float x, float y)
        {
            return new Vector2(x / texture.Width, y / texture.Height);
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
