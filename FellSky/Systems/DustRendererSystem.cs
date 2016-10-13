using Microsoft.Xna.Framework.Graphics;
using FellSky.Framework;
using FellSky.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace FellSky.Systems
{
    public class DustRendererSystem : Artemis.System.ProcessingSystem
    {
        private Texture2D _dustTexture;
        private FastSpriteBatch _batch;
        Vertex3CTN[] _quad = new Vertex3CTN[4];
        private int[] _quadIndices = new int[] { 0, 1, 2, 1, 3, 2 };

        public override void LoadContent()
        {
            var content = ServiceLocator.Instance.GetService<Microsoft.Xna.Framework.Content.ContentManager>();
            _dustTexture = content.Load<Texture2D>("Textures/dust");
            _batch = ServiceLocator.Instance.GetService<FastSpriteBatch>();
        }        

        public override void ProcessSystem()
        {
            var camera = EntityWorld.GetActiveCamera();
            var viewRect = camera.GetViewRect(1.0f);

            Vertex3CTN vtx = new Vertex3CTN();
            vtx.Color = Color.White;
            vtx.Normal = Vector3.Forward;
            var origSampleState = _batch.GraphicsDevice.SamplerStates[0];
            _batch.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            vtx.Position = new Vector3(viewRect.Left, viewRect.Top, 0);
            vtx.TextureCoords = new Vector2(-viewRect.Left, -viewRect.Top);
            _quad[0] = vtx;
            vtx.Position = new Vector3(viewRect.Left, viewRect.Bottom, 0);
            vtx.TextureCoords = new Vector2(-viewRect.Left, -viewRect.Bottom);
            _quad[1] = vtx;
            vtx.Position = new Vector3(viewRect.Right, viewRect.Top, 0);
            vtx.TextureCoords = new Vector2(-viewRect.Right, -viewRect.Top);
            _quad[2] = vtx;
            vtx.Position = new Vector3(viewRect.Right, viewRect.Bottom, 0);
            vtx.TextureCoords = new Vector2(-viewRect.Right, -viewRect.Bottom);
            _quad[3] = vtx;

            Matrix matrix = Matrix.Identity;
            _batch.Begin(effect: camera.SpriteBatchBasicEffect);
            _batch.DrawVertices(_dustTexture, _quad, _quadIndices, ref matrix);
            _batch.End();

            _batch.GraphicsDevice.SamplerStates[0] = origSampleState;
        }
    }
}
