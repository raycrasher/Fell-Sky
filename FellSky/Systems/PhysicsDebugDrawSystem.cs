using FellSky.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics.Common;
using FellSky.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;

namespace FellSky.Systems
{
    public class PhysicsDebugDrawSystem : Artemis.System.ProcessingSystem
    {
        private string _cameraTag;
        FarseerPhysics.DebugView.DebugViewXNA _debugView;
        public PhysicsDebugDrawSystem(string cameraTag)
        {
            _cameraTag = cameraTag;
        }

        public override void LoadContent()
        {
            var world = EntityWorld.SystemManager.GetSystem<PhysicsSystem>().PhysicsWorld;
            _debugView = new FarseerPhysics.DebugView.DebugViewXNA(world);
            base.LoadContent();
        }

        public override void ProcessSystem()
        {
            var camera = EntityWorld.GetCamera(_cameraTag);
            _debugView.RenderDebugData(camera.ProjectionMatrix, camera.GetViewMatrix(1.0f));
            
        }

       
    }
}
