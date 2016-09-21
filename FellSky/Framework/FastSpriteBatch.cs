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
        private int[] _quadIndices = new int[]{ 0, 1, 2, 1, 3, 2 };

        public int NumVertices => _vertexCount;
        public int NumIndices => _indexCount;

        public FastSpriteBatch(int initialVertexCount = 10000)
        {
            _vertices = new Vertex3CTN[initialVertexCount];
            _indices = new int[_vertices.Length * 3 / 2];
        }

        public void Reset()
        {
            _vertexCount = 0;
            _indexCount = 0;
        }

        public void Draw(SpriteComponent sprite, ref Matrix transform, Color? color=null, Vector3? normal=null, SpriteEffects flip = SpriteEffects.None)
        {
            Vertex3CTN vtx;
            vtx.Color = color ?? Color.White;
            vtx.Normal = normal ?? Vector3.Forward;
            
            var tex = sprite.Texture;

            Vector2 texCoordLT = GetUV(tex, sprite.TextureRect.Left, sprite.TextureRect.Top);
            Vector2 texCoordBR = GetUV(tex, sprite.TextureRect.Right, sprite.TextureRect.Bottom);

            bool changeOrder = false;

            if ((flip & SpriteEffects.FlipHorizontally) != 0)
            {
                var temp = texCoordBR.X;
                texCoordBR.X = texCoordLT.X;
                texCoordLT.X = temp;
                changeOrder = true;
            }

            if ((flip & SpriteEffects.FlipVertically) != 0)
            {
                var temp = texCoordBR.Y;
                texCoordBR.Y = texCoordLT.Y;
                texCoordLT.Y = temp;
                changeOrder ^= true;
            }

            vtx.Position = Vector3.Zero;
            vtx.TextureCoords = texCoordLT;
            _quadVertices[changeOrder? 3 : 0] = vtx;
            vtx.Position = new Vector3(0, sprite.TextureRect.Height, 0);
            vtx.TextureCoords = new Vector2(texCoordLT.X, texCoordBR.Y);
            _quadVertices[changeOrder ? 2 : 1] = vtx;
            vtx.Position = new Vector3(sprite.TextureRect.Width, 0, 0);
            vtx.TextureCoords = new Vector2(texCoordBR.X, texCoordLT.Y);
            _quadVertices[changeOrder ? 1 : 2] = vtx;
            vtx.Position = new Vector3(sprite.TextureRect.Width, sprite.TextureRect.Height, 0);
            vtx.TextureCoords = texCoordBR;
            _quadVertices[changeOrder ? 0 : 3] = vtx;

            

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

        public void Render(GraphicsDevice device, Effect effect)
        {
            foreach(var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _vertices, 0, _vertexCount, _indices, 0, _indexCount / 3);
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
