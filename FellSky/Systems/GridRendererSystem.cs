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

namespace FellSky.Components
{
    public class GridRendererSystem : Artemis.System.EntityComponentProcessingSystem<GridComponent>
    {
        private CameraComponent _camera;
        private SpriteBatch _spriteBatch;

        public string CameraTag { get; set; }

        public GridRendererSystem(SpriteBatch spriteBatch, string _cameraTag)
        {
            CameraTag = _cameraTag;
            _spriteBatch = spriteBatch;
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

            _spriteBatch.Begin(transformMatrix: matrix);

            for(float x = (float)Math.Round(rect.Left / grid.GridSize.X, MidpointRounding.AwayFromZero) * grid.GridSize.X; x < rect.Right; x+= grid.GridSize.X)
                _spriteBatch.DrawLine(x, rect.Top, x, rect.Bottom, grid.GridColor);

            for (float y = (float)Math.Round(rect.Top / grid.GridSize.Y, MidpointRounding.AwayFromZero) * grid.GridSize.Y; y < rect.Bottom; y += grid.GridSize.Y)
                _spriteBatch.DrawLine(rect.Left, y, rect.Right, y, grid.GridColor);

            _spriteBatch.End();
        }
    }
}
