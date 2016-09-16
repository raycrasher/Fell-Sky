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
    public class CustomSpriteBatch
    {
        private Vertex3N[] _vertices;
        private int[] _indices;
        private int _iVertex, _iIndex;
        private Effect _effect;

        private Vertex3N[] _quadVertices = new Vertex3N[4];
        private int[] _quadIndices = new int[] { 0, 1, 2, 1, 3, 2 };

        public CustomSpriteBatch(int initialVertexCount = 10000)
        {
            _vertices = new Vertex3N[initialVertexCount];
            _indices = new int[initialVertexCount];
        }

        public void Begin(Effect effect)
        {
            _effect = effect;
        }

        public void End()
        {

        }

        public void Draw(SpriteComponent sprite, ref Matrix transform, Color color)
        {
            Vertex3N vtx;
            vtx.Color = color;
            vtx.Normal = Vector3.Forward;

            vtx.Position = Vector3.Zero;
            vtx.TextureCoords = new Vector2(sprite.TextureRect.Left, sprite.TextureRect.Top);
            _quadVertices[0] = vtx;

            vtx.Position = new Vector3(0, sprite.TextureRect.Height, 0);
            vtx.TextureCoords = new Vector2(sprite.TextureRect.Left, sprite.TextureRect.Bottom);
            _quadVertices[1] = vtx;
            vtx.Position = new Vector3(sprite.TextureRect.Width, 0, 0);
            vtx.TextureCoords = new Vector2(sprite.TextureRect.Right, sprite.TextureRect.Top);
            _quadVertices[2] = vtx;
            vtx.Position = new Vector3(sprite.TextureRect.Width, sprite.TextureRect.Height, 0);
            vtx.TextureCoords = new Vector2(sprite.TextureRect.Right, sprite.TextureRect.Bottom);
            _quadVertices[3] = vtx;

            Draw(sprite.Texture, _quadVertices, _quadIndices, ref transform);
        }

        public void Draw(SpriteComponent sprite, ref Matrix transform, Color color, Vertex3N normal)
        {
            Vertex3 vtx;
            vtx.Color = color;

            vtx.Position = Vector3.Zero;
            vtx.TextureCoords = new Vector2(sprite.TextureRect.X, sprite.TextureRect.Y);

        }

        public void Draw(Texture2D texture, Vertex3N[] vertices, int[] indices, ref Matrix transform)
        {
            for (int i = 0; i < vertices.Length; i++) {
                var vtx = vertices[i];

                Vector3.Transform(ref vtx.Position, ref transform, out vtx.Position);
                _vertices[_iVertex++] = vtx;
            }
        }
    }
}
