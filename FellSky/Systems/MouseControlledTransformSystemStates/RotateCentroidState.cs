using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using FellSky.Components;

namespace FellSky.Systems.MouseControlledTransformSystemStates
{
    public class RotateCentroidState: IMouseControlledTransformSystemState
    {
        private Vector2 _centroid;
        private Vector2? _rotateOffset;
        private Entity[] _entities;
        private Dictionary<Entity, Transform> _initialTransforms;
        private Vector2 _origin;

        public float SnapAmount { get; set; }
        public bool IsSnapEnabled { get; set; }
        public Axis Constraint { get; set; }

        public void Apply()
        {            
        }

        public void Cancel()
        {
            foreach(var entity in _entities)
            {
                entity.GetComponent<Transform>().CopyValuesFrom(_initialTransforms[entity]);
            }
        }

        public void Start(IEnumerable<Entity> entities, Vector2 worldMousePosition)
        {
            _origin = worldMousePosition;
            _initialTransforms = entities.ToDictionary(e=>e, e => e.GetComponent<Transform>().Clone());
            _entities = entities.ToArray();
            _centroid = _entities.Aggregate(Vector2.Zero, (current, input) => current + input.GetComponent<Transform>().Position) / _entities.Length;
        }

        public void Transform(Vector2 worldMousePosition)
        {
            if (Vector2.DistanceSquared(worldMousePosition, _centroid) > 40)
                _rotateOffset = _rotateOffset ?? worldMousePosition;
            
            if (_rotateOffset != null)
            {
                foreach (var entity in _entities)
                {
                    var transform = entity.GetComponent<Transform>();
                    var control = entity.GetComponent<MouseControlledTransformComponent>();
                    var initialAngle = (_rotateOffset.Value - _centroid).GetAngleRadians();
                    var angle = (worldMousePosition - _centroid).GetAngleRadians();

                    if(IsSnapEnabled)
                    {
                        angle = ((int)angle / SnapAmount) * SnapAmount;
                    }

                    var initialTransform = _initialTransforms[entity];
                    var newMatrix =
                        Matrix.CreateScale(new Vector3(initialTransform.Scale, 0))
                        //* Matrix.CreateRotationZ(control.InitialTransform.Rotation)
                        * Matrix.CreateTranslation(new Vector3(initialTransform.Position, 0))
                        * Matrix.CreateTranslation(new Vector3(-_centroid, 0))
                        * Matrix.CreateRotationZ(MathHelper.WrapAngle(angle - initialAngle))
                        * Matrix.CreateTranslation(new Vector3(_centroid, 0));

                    Vector2 position, scale;
                    float rotation;
                    Utilities.DecomposeMatrix2D(ref newMatrix, out position, out rotation, out scale);
                    transform.Position = position;
                    transform.Rotation = initialTransform.Rotation + MathHelper.WrapAngle(angle - initialAngle);
                    transform.Scale = initialTransform.Scale;
                }
            }
        }
    }
}
