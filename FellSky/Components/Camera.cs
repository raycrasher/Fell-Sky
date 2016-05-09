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
        }

        public Transform Transform { get; set; }
            = new Transform();

        public float Zoom { get; set; } = 1;
        public Vector2 ScreenSize { get; set; }
        public Matrix ProjectionMatrix
        {
            get {
                var vp = Device.Viewport;
                Matrix projection;
                Matrix.CreateOrthographicOffCenter(0, vp.Width, vp.Height, 0, -1, 1, out projection);
                return projection;
            }
        }
        public GraphicsDevice Device { get; set; }

        public Matrix GetViewMatrix(float parallax)
        {
            float scaleFactor = 1 / (parallax + Zoom) * 2;

            // To add parallax, simply multiply it by the position
            return Matrix.CreateTranslation(new Vector3(-(Transform.Position * parallax), 0.0f)) *
                // The next line has a catch. See note below.
                Matrix.CreateTranslation(new Vector3(-Transform.Origin, 0.0f)) *
                Matrix.CreateRotationZ(Transform.Rotation) *
                Matrix.CreateScale(scaleFactor, scaleFactor, 1) *
                Matrix.CreateTranslation(new Vector3(Transform.Origin, 0.0f)) *
                Matrix.CreateTranslation(new Vector3(ScreenSize / 2, 0f));
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
