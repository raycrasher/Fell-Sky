using Artemis.Interface;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public class AnimationComponent: IComponent
    {
        private Transform _transform = new Transform();

        public bool IsFinished => CurrentTime >= 1f;
        public float CurrentTime;

        public Transform Transform { get {
                _transform.Position = PositionAnimator.Current;
                _transform.Rotation = RotationAnimator.Current;
                _transform.Scale = ScaleAnimator.Current;
                return _transform;
        }}

        public Color Color => ColorAnimator.Current;
        public float Alpha => AlphaAnimator.Current;

        public IEnumerator<Vector2> PositionAnimator;
        public IEnumerator<float> RotationAnimator;
        public IEnumerator<Vector2> ScaleAnimator;
        public IEnumerator<Color> ColorAnimator;
        public IEnumerator<float> AlphaAnimator;
    }
}
