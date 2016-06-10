﻿using System;
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
using FellSky.Game.Campaign;

namespace FellSky.Systems
{
    
    public class PlayerShipControlSystem : Artemis.System.TagSystem
    {
        public const string PlayerShipTag = "PlayerShip";
        private IKeyboardService _keyboard;
        private IMouseService _mouse;

        public PlayerControls Controls { get; set; } = new PlayerControls();

        public PlayerShipControlSystem()
            : base(PlayerShipTag)
        {
            _mouse = ServiceLocator.Instance.GetService<IMouseService>();
            _keyboard = ServiceLocator.Instance.GetService<IKeyboardService>();
        }

        public override void Process(Entity entity)
        {
            UpdateMovement(entity);
            UpdateTurrets(entity);
        }

        private void UpdateTurrets(Entity shipEntity)
        {
            var shipComponent = shipEntity.GetComponent<ShipComponent>();
            var shipXform = shipEntity.GetComponent<Transform>();
            var shipMatrix = shipXform.Matrix;
            var camera = EntityWorld.GetActiveCamera();
            Vector2 worldMousePos = camera.ScreenToCameraSpace(_mouse.ScreenPosition);

            foreach (var turretEntity in shipComponent.Turrets)
            {
                var turret = turretEntity.GetComponent<TurretComponent>();
                var turretXform = turretEntity.GetComponent<Transform>();                
                
                Vector2 transformedMousePos;
                Vector2 turretPos = turretXform.Position;
                Vector2.Transform(ref turretPos, ref shipMatrix, out transformedMousePos);

                var offset = transformedMousePos - worldMousePos;

                turret.DesiredRotation = offset.GetAngleRadians() - shipXform.Rotation - turretXform.Rotation + MathHelper.PiOver2;
            }
        }

        private void UpdateMovement(Entity entity)
        {
            var shipComponent = entity.GetComponent<ShipComponent>();
            var xform = entity.GetComponent<Transform>();
            
            Vector2 linearThrustVector = Vector2.Zero;
            float heading = xform.Rotation;
            AngularDirection torqueDirection = AngularDirection.None;

            if (Controls.OrthogonalMovement)
            {
                if (_keyboard.IsKeyDown(Controls.UpKey)) linearThrustVector.Y = 1;
                else if (_keyboard.IsKeyDown(Controls.DownKey)) linearThrustVector.Y = -1;
                if (_keyboard.IsKeyDown(Controls.LeftKey)) linearThrustVector.X = -1;
                else if (_keyboard.IsKeyDown(Controls.RightKey)) linearThrustVector.X = 1;
            }
            else
            {
                if (_keyboard.IsKeyDown(Controls.UpKey)) linearThrustVector.X = 1;
                else if (_keyboard.IsKeyDown(Controls.DownKey)) linearThrustVector.X = -1;

                if (_keyboard.IsKeyDown(Controls.AltLeftKey)) linearThrustVector.Y = -1;
                else if (_keyboard.IsKeyDown(Controls.AltRightKey)) linearThrustVector.Y = 1;

                if (_keyboard.IsKeyDown(Controls.LeftKey)) torqueDirection = AngularDirection.CCW;
                else if (_keyboard.IsKeyDown(Controls.RightKey)) torqueDirection = AngularDirection.CW;

                shipComponent.AngularTorque = (int)torqueDirection;
            }

            if (Controls.MouseHeadingControl)
            {
                var camera = EntityWorld.GetActiveCamera();
                Vector2 worldMousePosition = camera.ScreenToCameraSpace(_mouse.ScreenPosition);
                var angle = (xform.Position - worldMousePosition).GetAngleRadians();

            }

            shipComponent.AttemptBoost = _keyboard.IsKeyDown(Controls.BoostKey);
            shipComponent.LinearThrustVector = linearThrustVector;
        }
    }
    
}
