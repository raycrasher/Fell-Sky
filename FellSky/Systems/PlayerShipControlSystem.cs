using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using Microsoft.Xna.Framework;
using Artemis.Attributes;
using FellSky.Game.Ships;

using FellSky.Framework;
using FellSky.Components;
using FellSky.Services;

namespace FellSky.Systems
{
    
    public class PlayerShipControlSystem : Artemis.System.TagSystem
    {
        public const string PlayerShipTag = "PlayerShip";
        private IKeyboardService _keyboard;
        private IMouseService _mouse;

        public PlayerShipControlSystem(string cameraTag, IKeyboardService keyboard, IMouseService mouse)
            : base(PlayerShipTag)
        {
            CameraTag = cameraTag;
            _mouse = mouse;
            _keyboard = keyboard;
        }

        public string CameraTag { get; private set; }

        public override void Process(Entity entity)
        {
            var shipComponent = entity.GetComponent<ShipComponent>();
            var xform = entity.GetComponent<Transform>();
            var control = entity.GetComponent<PlayerControlsComponent>();
            Vector2 linearThrustVector = Vector2.Zero;
            float heading = xform.Rotation;
            AngularDirection torqueDirection = AngularDirection.None;

            if (control.OrthogonalMovement)
            {
                if (_keyboard.IsKeyDown(control.UpKey)) linearThrustVector.Y = -1;
                else if (_keyboard.IsKeyDown(control.DownKey)) linearThrustVector.Y = 1;
                if (_keyboard.IsKeyDown(control.LeftKey)) linearThrustVector.Y = -1;
                else if (_keyboard.IsKeyDown(control.RightKey)) linearThrustVector.X = 1;
            }
            else
            {
                if (_keyboard.IsKeyDown(control.UpKey)) linearThrustVector += heading.ToVector();
                else if (_keyboard.IsKeyDown(control.DownKey)) linearThrustVector += -heading.ToVector();

                if (_keyboard.IsKeyDown(control.AltLeftKey)) linearThrustVector += ((float)(heading + Math.PI / 2)).ToVector();
                else if (_keyboard.IsKeyDown(control.AltRightKey)) linearThrustVector += ((float)(heading - Math.PI / 2)).ToVector();

                if (_keyboard.IsKeyDown(control.LeftKey)) torqueDirection = AngularDirection.CCW;
                else if (_keyboard.IsKeyDown(control.RightKey)) torqueDirection = AngularDirection.CW;

                shipComponent.AngularTorque = (int)torqueDirection * shipComponent.Ship.Handling.AngularTorque;
            }

            if (control.MouseHeadingControl)
            {
                var camera = EntityWorld.GetCamera(CameraTag);
                Vector2 worldMousePosition = Vector2.Transform(_mouse.ScreenPosition, camera.GetViewMatrix(1.0f));
                var angle = (xform.Position - worldMousePosition).ToAngleRadians();

            }

            shipComponent.AttemptBoost = _keyboard.IsKeyDown(control.BoostKey);
            shipComponent.LinearThrustVector = linearThrustVector;
        }
    }
    
}
