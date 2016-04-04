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
    public class ScaleState : IMouseControlledTransformSystemState
    {
        private Vector2 _centroid;
        private Entity[] _entities;
        private Dictionary<Entity, Transform> _initialTransforms;
        private Vector2 _origin;
        private Vector2 _originToCentroidOffset;

        public bool IsSnapEnabled { get; set; }
        public float SnapAmount { get; set; }
        public Axis Constraint { get; set; }

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
            _originToCentroidOffset = _origin - _centroid;
        }

        public void Transform(Vector2 worldMousePosition)
        {
            var offset = worldMousePosition - _origin;
            var centerOffset = _centroid - worldMousePosition;
            var factor = (worldMousePosition - _centroid) / (_origin - _centroid);
            var newScale = IsSnapEnabled ? new Vector2(Math.Max(factor.X, factor.Y), Math.Max(factor.X, factor.Y)) : factor;
            var matrix = Matrix.CreateScale(new Vector3(newScale, 1));

            foreach(var entity in _entities){
                var initialXform = _initialTransforms[entity];
                var xform = entity.GetComponent<Transform>();
                var partVector = initialXform.Position - _centroid;
                var newPos = Vector2.Transform(partVector, matrix) + _centroid;
                var indivScale = newScale;
                if (Constraint == Axis.X) indivScale.Y = indivScale.X;
                else if (Constraint == Axis.Y) indivScale.X = indivScale.Y;

                xform.Scale = initialXform.Scale * indivScale;
                xform.Position = newPos;
            }
        }
    }
}
