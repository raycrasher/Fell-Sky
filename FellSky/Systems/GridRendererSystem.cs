using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using Artemis.Attributes;
using Microsoft.Xna.Framework.Graphics;

using FellSky.Components;
using FellSky.Framework;
using Microsoft.Xna.Framework;

namespace FellSky.Systems
{
    public class GridRendererSystem : Artemis.System.EntityComponentProcessingSystem<GridComponent>
    {
        private CameraComponent _camera;
        private BasicEffect _effect;
        private GraphicsDevice _device;
        private Vertex[] _vertices = new Vertex[200];

        public string CameraTag { get; set; }

        public GridRendererSystem(GraphicsDevice device, string _cameraTag)
        {
            CameraTag = _cameraTag;
            _effect = new BasicEffect(device);
            _device = device;
        }

        protected override void Begin()
        {
            base.Begin();
            _camera = EntityWorld.GetCamera(CameraTag);
        }

        public override void Process(Entity entity, GridComponent grid)
        {
            var matrix = _camera.GetViewMatrix(grid.Parallax);
            var rect = _camera.GetViewRect(grid.Parallax);

            Matrix projection = _camera.ProjectionMatrix;
            Matrix.Multiply(ref matrix, ref projection, out projection);

            _effect.VertexColorEnabled = true;
            _effect.World = projection;

            int index = 0;
            int numPrimitives = 0;
            

            for(float x = (float)Math.Round(rect.Left / grid.GridSize.X, MidpointRounding.AwayFromZero) * grid.GridSize.X; x < rect.Right; x+= grid.GridSize.X)
            {
                if (index > _vertices.Length)
                    Array.Resize(ref _vertices, (int)(_vertices.Length * 1.5));
                _vertices[index++] = new Vertex { Position = new Vector2(x, rect.Top), Color = grid.GridColor };
                _vertices[index++] = new Vertex { Position = new Vector2(x, rect.Bottom), Color = grid.GridColor };
                numPrimitives++;
            }
            
            for (float y = (float)Math.Round(rect.Top / grid.GridSize.Y, MidpointRounding.AwayFromZero) * grid.GridSize.Y; y < rect.Bottom; y += grid.GridSize.Y)
            {
                if (index > _vertices.Length - 1)
                    Array.Resize(ref _vertices, (int)(_vertices.Length * 1.5));
                _vertices[index++] = new Vertex { Position = new Vector2(rect.Left, y), Color = grid.GridColor };
                _vertices[index++] = new Vertex { Position = new Vector2(rect.Right, y), Color = grid.GridColor };
                numPrimitives++;

            }
            _effect.CurrentTechnique.Passes[0].Apply();
            _device.DrawUserPrimitives(PrimitiveType.LineList, _vertices, 0, numPrimitives);
        }
    }
}
