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
using Microsoft.Xna.Framework.Content;
using FellSky.Components;

namespace FellSky.Systems
{
    public class PhysicsDebugDrawSystem : Artemis.System.ProcessingSystem
    {
        FarseerPhysics.DebugView.DebugViewXNA _debugView;

        public override void LoadContent()
        {
            var world = EntityWorld.SystemManager.GetSystem<PhysicsSystem>().PhysicsWorld;
            _debugView = new FarseerPhysics.DebugView.DebugViewXNA(world);
            _debugView.LoadContent(ServiceLocator.Instance.GetService<GraphicsDevice>(), ServiceLocator.Instance.GetService<ContentManager>());
            _debugView.Flags = FarseerPhysics.DebugViewFlags.Shape | FarseerPhysics.DebugViewFlags.CenterOfMass;

            var keyboard = ServiceLocator.Instance.GetService<IKeyboardService>();
            keyboard.KeyDown += key =>
            {
                if (key == Microsoft.Xna.Framework.Input.Keys.F11) IsEnabled = !IsEnabled;
            };
            IsEnabled = false;
            base.LoadContent();
        }

        public override void ProcessSystem()
        {
            var camera = EntityWorld.TagManager.GetEntity(Constants.ActiveCameraTag).GetComponent<Camera>();
            _debugView.RenderDebugData(camera.ProjectionMatrix, Matrix.CreateScale(1f / Constants.PhysicsUnitScale) * camera.GetViewMatrix(1.0f));
        }

       
    }
}
