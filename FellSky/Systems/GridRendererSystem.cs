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
using FellSky.Services;

namespace FellSky.Systems
{
    public class GridRendererSystem : Artemis.System.EntityComponentProcessingSystem<GridComponent>
    {
        private BasicEffect _effect;
        private GraphicsDevice _device;
        private Vertex2CT[] _vertices = new Vertex2CT[200];

        public GridRendererSystem()
        {
            _device = ServiceLocator.Instance.GetService<GraphicsDevice>();
            _effect = new BasicEffect(_device);
        }


        public override void Process(Entity entity, GridComponent grid)
        {
            var camera = EntityWorld.TagManager.GetEntity(Constants.ActiveCameraTag).GetComponent<Camera>();
            var matrix = camera.GetViewMatrix(grid.Parallax);
            var rect = camera.GetViewRect(grid.Parallax);

            Matrix projection = camera.ProjectionMatrix;
            Matrix.Multiply(ref matrix, ref projection, out projection);

            _effect.VertexColorEnabled = true;
            _effect.World = projection;

            int index = 0;
            int numPrimitives = 0;
            

            for(float x = (float)Math.Round(rect.Left / grid.GridSize.X, MidpointRounding.AwayFromZero) * grid.GridSize.X; x < rect.Right; x+= grid.GridSize.X)
            {
                if (index >= _vertices.Length)
                    Array.Resize(ref _vertices, (int)(_vertices.Length * 1.5));
                _vertices[index++] = new Vertex2CT { Position = new Vector2(x, rect.Top), Color = grid.GridColor };
                _vertices[index++] = new Vertex2CT { Position = new Vector2(x, rect.Bottom), Color = grid.GridColor };
                numPrimitives++;
            }
            
            for (float y = (float)Math.Round(rect.Top / grid.GridSize.Y, MidpointRounding.AwayFromZero) * grid.GridSize.Y; y < rect.Bottom; y += grid.GridSize.Y)
            {
                if (index >= _vertices.Length - 1)
                    Array.Resize(ref _vertices, (int)(_vertices.Length * 1.5));
                _vertices[index++] = new Vertex2CT { Position = new Vector2(rect.Left, y), Color = grid.GridColor };
                _vertices[index++] = new Vertex2CT { Position = new Vector2(rect.Right, y), Color = grid.GridColor };
                numPrimitives++;

            }
            _effect.CurrentTechnique.Passes[0].Apply();
            _device.DrawUserPrimitives(PrimitiveType.LineList, _vertices, 0, numPrimitives);
        }
    }
}
