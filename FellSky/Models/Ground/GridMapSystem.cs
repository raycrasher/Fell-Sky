using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FellSky.Framework;
using FellSky.Components;

namespace FellSky.Ground
{
    public class GridMapSystem : Artemis.System.ProcessingSystem
    {
        public GridMapSystem(string cameraTag, string mapTag)
        {
            CameraTag = cameraTag;
            MapTag = mapTag;
            Camera = EntityWorld.GetCamera(cameraTag);
            //Map = EntityWorld.TagManager.GetEntity<GridMap>;
        }

        public CameraComponent Camera { get; private set; }
        public string CameraTag { get; private set; }
        public GridMap Map { get; private set; }
        public string MapTag { get; private set; }

        public override void ProcessSystem()
        {
            
        }
    }
}
