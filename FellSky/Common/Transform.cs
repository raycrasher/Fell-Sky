using Microsoft.Xna.Framework;

namespace FellSky.Common
{
    public interface ITransform
    {
        Vector2 Position { get; set; }
        float Rotation { get; set; }
        Vector2 Scale { get; set; }

        Matrix GetMatrix();
    }

    public struct Transform : ITransform
    {
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; }

        public Matrix GetMatrix()
        {
            return Matrix.CreateScale(new Vector3(Scale, 1)) * Matrix.CreateRotationZ(Rotation) * Matrix.CreateTranslation(new Vector3(Position, 0));
        }
    }
}
