using Artemis.Interface;
using FellSky.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Graphics
{
    public class Camera2D : IComponent
    {
        public const string PlayerCameraName = "PlayerCamera";

        public Transform Transform { get; set; }
            = new Transform();

        public float Zoom { get; set; } = 1;

        public Matrix GetViewMatrix(float parallax)
        {
            // To add parallax, simply multiply it by the position
            return Matrix.CreateTranslation(new Vector3(-Transform.Position * parallax, 0.0f)) *
                // The next line has a catch. See note below.
                Matrix.CreateTranslation(new Vector3(-Transform.Origin, 0.0f)) *
                Matrix.CreateRotationZ(Transform.Rotation) *
                Matrix.CreateScale(Zoom, Zoom, 1) *
                Matrix.CreateTranslation(new Vector3(Transform.Origin, 0.0f));
        }
    }
}
