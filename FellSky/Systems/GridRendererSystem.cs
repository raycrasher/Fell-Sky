using FellSky.EntityComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using Artemis.Attributes;
using Microsoft.Xna.Framework.Graphics;
using FellSky.Graphics;

namespace FellSky.Systems
{
    [ArtemisEntitySystem(ExecutionType = Artemis.Manager.ExecutionType.Synchronous, GameLoopType = Artemis.Manager.GameLoopType.Draw, Layer = 8)]
    public class GridRendererSystem : Artemis.System.EntityComponentProcessingSystem<GridComponent>
    {
        private Camera2D _camera;
        private SpriteBatch _spriteBatch;

        public override void LoadContent()
        {
            base.LoadContent();
            _spriteBatch = BlackBoard.GetService<SpriteBatch>();
        }

        protected override void Begin()
        {
            base.Begin();
            _camera = BlackBoard.GetEntry<Camera2D>(Camera2D.PlayerCameraName);
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
