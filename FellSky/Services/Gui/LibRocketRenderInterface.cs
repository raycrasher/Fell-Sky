using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using FellSky.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FellSky.Services.Gui
{
    unsafe class LibRocketRenderInterface : LibRocketNet.RenderInterface
    {
        public static int NumStartingVertices = 400;
        public static int NumStartingIndices = 400;

        public bool UseVBO { get; private set; }

        private Vertex2[] _vertices;
        private int[] _indices;


        public GraphicsDevice GraphicsDevice;

        

        bool _scissor = false;

        Dictionary<IntPtr, Geometry> _geometries = new Dictionary<IntPtr, Geometry>();
        Dictionary<IntPtr, Texture2D> _textures = new Dictionary<IntPtr, Texture2D>();
        BasicEffect _effect;
        RasterizerState _rStateNoScissor = new RasterizerState { CullMode = CullMode.None, ScissorTestEnable = false };
        RasterizerState _rStateScissor = new RasterizerState { CullMode = CullMode.None, ScissorTestEnable = true };
        private ContentManager _content;
        private static object LibRocketTextureTag = new object();

        public LibRocketRenderInterface(GraphicsDevice device, ContentManager content, bool useVbo = false)
        {
            _content = content;
            GraphicsDevice = device;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            _vertices = new Vertex2[NumStartingVertices];
            _indices = new int[NumStartingIndices];
            _effect = new BasicEffect(device);
            _effect.VertexColorEnabled = true;

            var w = device.Viewport.Width;
            var h = device.Viewport.Height;

            _effect.Projection = Matrix.CreateOrthographic(w, -h, -1, 1);
            _effect.VertexColorEnabled = true;
            _effect.View = Matrix.CreateLookAt(new Vector3(w / 2, h / 2, 1), new Vector3(w / 2, h / 2, 0), Vector3.Up);

            UseVBO = useVbo;
        }

        protected override IntPtr CompileGeometry(LibRocketNet.Vertex* vertices, int num_vertices, int* indices, int num_indices, IntPtr texture)
        {
            if (!UseVBO) return IntPtr.Zero;

            var geom = new Geometry();
            if (texture != IntPtr.Zero) geom.Texture = _textures[texture];

            var vbuf = new Vertex2[num_vertices];
            var ibuf = new int[num_indices];

            CopyVertices(vertices, vbuf, num_vertices);
            Marshal.Copy((IntPtr)indices, ibuf, 0, num_indices);

            geom.Vertices = new VertexBuffer(GraphicsDevice, Vertex2.VertexDeclarationStatic, num_vertices, BufferUsage.None);
            geom.Vertices.SetData(vbuf);

            geom.Indices = new IndexBuffer(GraphicsDevice, IndexElementSize.ThirtyTwoBits, num_indices, BufferUsage.None);
            geom.Indices.SetData(ibuf);

            var handle = (IntPtr)geom.GetHashCode();
            _geometries[handle] = geom;
            return handle;
        }

        protected override void RenderGeometry(LibRocketNet.Vertex* vertices, int num_vertices, int* indices, int num_indices, IntPtr texture, LibRocketNet.Vector2f translation)
        {
            var rstate = GraphicsDevice.RasterizerState;

            var blendState = BlendState.NonPremultiplied;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            if (_vertices.Length < num_vertices) Array.Resize(ref _vertices, num_vertices);
            if (_indices.Length < num_indices) Array.Resize(ref _indices, num_indices);

            CopyVertices(vertices, _vertices, num_vertices);
            Marshal.Copy((IntPtr)indices, _indices, 0, num_indices);

            _effect.World = Matrix.CreateTranslation(translation.X, translation.Y, 0);

            if (texture != IntPtr.Zero)
            {
                _effect.TextureEnabled = true;
                _effect.Texture = _textures[texture];
                if (_effect.Texture.Tag == LibRocketTextureTag)
                    blendState = BlendState.NonPremultiplied;
                else
                    blendState = BlendState.AlphaBlend;
            }
            else
            {
                _effect.TextureEnabled = false;
                _effect.Texture = null;
            }

            GraphicsDevice.BlendState = blendState;
            GraphicsDevice.RasterizerState = _scissor ? _rStateScissor : _rStateNoScissor;

            foreach (var pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    _vertices,
                    0,
                    num_vertices,
                    _indices,
                    0,
                    num_indices / 3,
                    Vertex2.VertexDeclarationStatic);
            }
            GraphicsDevice.RasterizerState = rstate;
        }

        protected override void RenderCompiledGeometry(IntPtr geometry, LibRocketNet.Vector2f translation)
        {
            if (!UseVBO) return;
            var rstate = GraphicsDevice.RasterizerState;
            var blendState = BlendState.NonPremultiplied;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            var geom = _geometries[geometry];
            if (geom.Texture != null)
            {
                _effect.TextureEnabled = true;
                _effect.Texture = geom.Texture;
                if (_effect.Texture.Tag == LibRocketTextureTag)
                    blendState = BlendState.NonPremultiplied;
                else
                    blendState = BlendState.AlphaBlend;
            }
            else
            {
                _effect.TextureEnabled = false;
                _effect.Texture = null;
            }

            _effect.World = Matrix.CreateTranslation(translation.X, translation.Y, 0);
            GraphicsDevice.SetVertexBuffer(geom.Vertices);
            GraphicsDevice.Indices = geom.Indices;
            GraphicsDevice.BlendState = blendState;
            GraphicsDevice.RasterizerState = _scissor ? _rStateScissor : _rStateNoScissor;

            foreach (var pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
#pragma warning disable CS0618
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, geom.Vertices.VertexCount, 0, geom.Indices.IndexCount);
#pragma warning restore CS0618
            }
            GraphicsDevice.RasterizerState = rstate;
        }

        protected override bool GenerateTexture(ref IntPtr texture_handle, byte* source, LibRocketNet.Vector2i source_dimensions)
        {
            var tex = new Texture2D(GraphicsDevice, source_dimensions.X, source_dimensions.Y, false, SurfaceFormat.Color);
            int length = 4 * (int)source_dimensions.X * (int)source_dimensions.Y;
            var data = new byte[length];

            //MemCopy((IntPtr)source, data, length);
            Marshal.Copy((IntPtr)source, data, 0, length);

            tex.SetData(data);

            texture_handle = (IntPtr)tex.GetHashCode();
            _textures[texture_handle] = tex;
            tex.Tag = LibRocketTextureTag;

            return true;
        }

        protected override bool LoadTexture(ref IntPtr texture_handle, ref LibRocketNet.Vector2i texture_dimensions, string source)
        {
            const string ContentTag = "content:";
            try
            {
                Texture2D tex;
                if (source.StartsWith(ContentTag))
                {
                    source = source.Substring(ContentTag.Length);
                    tex = _content.Load<Texture2D>(source);
                }
                else
                {
                    tex = _content.Load<Texture2D>(source);
                    tex.Tag = LibRocketTextureTag;
                }

                texture_dimensions = new LibRocketNet.Vector2i(tex.Width, tex.Height);
                texture_handle = (IntPtr)tex.GetHashCode();
                _textures[texture_handle] = tex;

            }
            catch (Exception ex)
            {
                Console.WriteLine("GUI: Error loading texture: {0}", ex.Message);
                texture_handle = IntPtr.Zero;
                return false;
            }
            return true;
        }

        protected override void ReleaseCompiledGeometry(IntPtr geometry)
        {
            _geometries[geometry].Dispose();
            _geometries.Remove(geometry);
        }

        protected override void ReleaseTexture(IntPtr texture)
        {
            var tex = _textures[texture];
            if (tex.Tag == LibRocketTextureTag)
            {
                tex.Dispose();
            }
            _textures.Remove(texture);
        }

        protected override void SetScissorRegion(int x, int y, int width, int height)
        {
            GraphicsDevice.ScissorRectangle = new Rectangle(x, y, width, height);
        }

        protected override void EnableScissorRegion(bool enable)
        {
            _scissor = enable;
        }

        //protected override float GetHorizontalTexelOffset()
        //{
        //    return 0.5f;
        //}
        //
        //protected override float GetVerticalTexelOffset()
        //{
        //    return 0.5f;
        //}
    }
}
