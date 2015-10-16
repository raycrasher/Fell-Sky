using FellSky.EntityComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using Microsoft.Xna.Framework;
using FellSky.EntitySystems;
using Artemis.Attributes;
using FellSky.Mechanics.Ships;

namespace FellSky.SpaceScene.Systems
{
    [ArtemisEntitySystem(ExecutionType = Artemis.Manager.ExecutionType.Synchronous, GameLoopType = Artemis.Manager.GameLoopType.Update)]
    public class PlayerShipControlSystem : Artemis.System.EntityComponentProcessingSystem<PlayerShipControllerComponent, Ship, ManeuverableComponent, Transform>
    {
        public override void Process(Entity entity, PlayerShipControllerComponent control, Ship ship, ManeuverableComponent maneuver, Transform xform)
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
                var camera = BlackBoard.GetEntry<Camera2D>("PlayerCamera");
                Vector2 worldMousePosition = Vector2.Transform(Game.Mouse.ScreenPosition, camera.CurrentMatrix);
                var angle = (xform.Position - worldMousePosition).ToAngleRadians();
                
            }

            maneuver.AttemptBoost = Game.Keyboard.IsKeyDown(control.BoostKey);
            
            maneuver.LinearThrustVector = linearThrustVector;
        }
    }
}
