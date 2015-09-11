using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FellSky.EntitySystems;
using FellSky.Mechanics.Ground;

namespace FellSky.GroundScene.Systems
{
    public class GridMapSystem : Artemis.System.ProcessingSystem
    {
        public GridMapSystem()
        {
            Camera = BlackBoard.GetEntry<Camera2D>("Camera");
            Map = BlackBoard.GetEntry<GridMap>("GridMap");
        }

        public Camera2D Camera { get; private set; }
        public GridMap Map { get; private set; }

        public override void ProcessSystem()
        {
            throw new NotImplementedException();
        }
    }
}
