using Microsoft.Xna.Framework.Graphics;
using System;

namespace FellSky.Framework
{
    class Geometry : IDisposable
    {
        bool _isDisposed = false;
        public VertexBuffer Vertices;
        public IndexBuffer Indices;
        public Texture2D Texture;
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                Vertices.Dispose();
                Indices.Dispose();
                GC.SuppressFinalize(this);
            }
        }
    }
}
