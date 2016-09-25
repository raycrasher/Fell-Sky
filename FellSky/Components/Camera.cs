﻿using System;
using Artemis.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FellSky.Components
{
    public class Camera : IComponent
    {
        public Camera(GraphicsDevice device)
        {
            Device = device;
            ScreenSize = new Vector2(device.Viewport.Width, device.Viewport.Height);
            
            _spriteBatchBasicEffect = new BasicEffect(device);
            _spriteBatchBasicEffect.VertexColorEnabled = true;
            _spriteBatchBasicEffect.TextureEnabled = true;

        }

        public Transform Transform { get; set; }
            = new Transform();

        public float Zoom { get; set; } = 1;
        public Vector2 ScreenSize { get; set; }
        public Matrix ProjectionMatrix => Matrix.CreateOrthographic(Device.Viewport.Width, Device.Viewport.Height, 1.0f, 10000.0f);
        public GraphicsDevice Device { get; set; }
        private BasicEffect _spriteBatchBasicEffect;

        public BasicEffect SpriteBatchBasicEffect
        {
            get {
                _spriteBatchBasicEffect.World = Matrix.Identity;
                _spriteBatchBasicEffect.View = GetViewMatrix(1.0f);
                _spriteBatchBasicEffect.Projection = ProjectionMatrix;
                return _spriteBatchBasicEffect;
            }
        }

        public Matrix GetViewMatrix(float parallax)
        {
            float scaleFactor = 1 / (parallax + Zoom) * 2;
            return Matrix.CreateLookAt(new Vector3(Transform.Position, -100), new Vector3(Transform.Position, 0), -Vector3.UnitY) * Matrix.CreateScale(1f / Zoom);            
        }

        public FloatRect GetViewRect(float parallax)
        {
            Vector2 upperLeft = Vector2.Zero, lowerRight = ScreenSize*2;
            return new FloatRect(ScreenToCameraSpace(upperLeft), ScreenToCameraSpace(lowerRight));
        }

        public Vector2 ScreenToCameraSpace(Vector2 screenCoords)
        {
            return Vector2.Transform(screenCoords, Matrix.Invert(GetViewMatrix(1.0f)));
        }
    }
}
