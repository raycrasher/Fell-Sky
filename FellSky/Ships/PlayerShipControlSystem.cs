using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using Microsoft.Xna.Framework;
using Artemis.Attributes;
using FellSky.Ships;
using FellSky.Graphics;
using FellSky.Framework;

namespace FellSky.Ships
{
    [ArtemisEntitySystem(ExecutionType = Artemis.Manager.ExecutionType.Synchronous, GameLoopType = Artemis.Manager.GameLoopType.Update)]
    public class PlayerShipControlSystem : Artemis.System.EntityComponentProcessingSystem<PlayerShipController, Ship, Maneuverable, Transform>
    {
        public override void Process(Entity entity, PlayerShipController control, Ship ship, Maneuverable maneuver, Transform xform)
        {
            Vector2 linearThrustVector = Vector2.Zero;
            float heading = xform.Rotation;
            AngularDirection torqueDirection = AngularDirection.None;

            if (control.OrthogonalMovement)
            {
                if (Game.Keyboard.IsKeyDown(control.UpKey)) linearThrustVector.Y = -1;
                else if (Game.Keyboard.IsKeyDown(control.DownKey)) linearThrustVector.Y = 1;
                if (Game.Keyboard.IsKeyDown(control.LeftKey)) linearThrustVector.Y = -1;
                else if (Game.Keyboard.IsKeyDown(control.RightKey)) linearThrustVector.X = 1;
            }
            else
            {
                if (Game.Keyboard.IsKeyDown(control.UpKey)) linearThrustVector += heading.ToVector();
                else if (Game.Keyboard.IsKeyDown(control.DownKey)) linearThrustVector += -heading.ToVector();

                if (Game.Keyboard.IsKeyDown(control.AltLeftKey)) linearThrustVector += ((float)(heading + Math.PI / 2)).ToVector();
                else if (Game.Keyboard.IsKeyDown(control.AltRightKey)) linearThrustVector += ((float)(heading - Math.PI / 2)).ToVector();

                if (Game.Keyboard.IsKeyDown(control.LeftKey)) torqueDirection = AngularDirection.CCW;
                else if (Game.Keyboard.IsKeyDown(control.RightKey)) torqueDirection = AngularDirection.CW;

                maneuver.Torque = (int)torqueDirection * ship.Handling.AngularTorque;
            }

            if (control.MouseHeadingControl)
            {
                var camera = BlackBoard.GetEntry<Camera2D>(Camera2D.PlayerCameraName);
                Vector2 worldMousePosition = Vector2.Transform(Game.Mouse.ScreenPosition, camera.GetViewMatrix(1.0f));
                var angle = (xform.Position - worldMousePosition).ToAngleRadians();
                
            }

            maneuver.AttemptBoost = Game.Keyboard.IsKeyDown(control.BoostKey);
            
            maneuver.LinearThrustVector = linearThrustVector;
        }
    }
}
