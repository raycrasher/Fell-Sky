using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using Microsoft.Xna.Framework;
using FellSky.Components;

namespace FellSky.Systems.MouseControlledTransformSystemStates
{
    public class TranslateState : IMouseControlledTransformSystemState
    {
        private Vector2 _centroid;
        private Entity[] _entities;
        private Dictionary<Entity, Transform> _initialTransforms;
        private Vector2 _origin;

        public void Apply()
        {
        }

        public void Cancel()
        {
            foreach (var entity in _entities)
            {
                entity.GetComponent<Transform>().CopyValuesFrom(_initialTransforms[entity]);
            }
        }

        public void Start(IEnumerable<Entity> entities, Vector2 worldMousePosition)
        {
            _origin = worldMousePosition;
            _initialTransforms = entities.ToDictionary(e => e, e => e.GetComponent<Transform>().Clone());
            _entities = entities.ToArray();
            _centroid = _entities.Aggregate(Vector2.Zero, (current, input) => current + input.GetComponent<Transform>().Position) / _entities.Length;
        }

        public void Transform(Vector2 worldMousePosition)
        {
            foreach (var entity in _entities)
            {
                var transform = entity.GetComponent<Transform>();
                var control = entity.GetComponent<MouseControlledTransformComponent>();
                var offset = worldMousePosition - _origin;
                var initialTransform = _initialTransforms[entity];
                Matrix? parentMatrix = entity.GetComponent<LocalTransformComponent>()?.ParentWorldMatrix;
                //transform.Position = Vector2.Transform(control.InitialTransform.Position + offset, control.TransformationMatrix);
                transform.Position = parentMatrix != null ? Vector2.Transform(initialTransform.Position + offset, parentMatrix.Value) : worldMousePosition;
            }
        }
    }
}
