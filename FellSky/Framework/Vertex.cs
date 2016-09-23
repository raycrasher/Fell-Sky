using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace FellSky
{
    /// <summary>
    /// A vertex with 2d position, color, and texture coordinates.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex2CT : IVertexType
    {
        public static readonly VertexDeclaration VertexDeclarationStatic = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
            new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
            );

#pragma warning disable 0649
        public VertexDeclaration VertexDeclaration => VertexDeclarationStatic;
        public Vector2 Position;
        public Color Color;
        public Vector2 TextureCoords;
#pragma warning restore 0649
    }

    /// <summary>
    /// A vertex with 3d position, color, and texture coordinates.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex3CT : IVertexType
    {
        public static readonly VertexDeclaration VertexDeclarationStatic = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(16, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
            );

#pragma warning disable 0649
        public VertexDeclaration VertexDeclaration => VertexDeclarationStatic;
        public Vector3 Position;
        public Color Color;
        public Vector2 TextureCoords;
#pragma warning restore 0649
    }

    /// <summary>
    /// A vertex with 3d position, color, texture coordinates, and normal.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex3CTN : IVertexType
    {
        public static readonly VertexDeclaration VertexDeclarationStatic = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(16, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(24, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
            );

#pragma warning disable 0649
        public VertexDeclaration VertexDeclaration => VertexDeclarationStatic;
        public Vector3 Position;
        public Color Color;
        public Vector2 TextureCoords;
        public Vector3 Normal;
#pragma warning restore 0649
    }
}
