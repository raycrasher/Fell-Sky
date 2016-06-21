using FellSky.Components;
using FellSky.Framework;
using FellSky.Services;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Ships
{
    public class PartAnimation
    {
        public List<Keyframe<Vector2>> Positions { get; set; } = new List<Keyframe<Vector2>>();
        public List<Keyframe<float>> Rotations { get; set; } = new List<Keyframe<float>>();
        public List<Keyframe<Vector2>> Scales { get; set; } = new List<Keyframe<Vector2>>();
        public List<Keyframe<Color>> Colors { get; set; } = new List<Keyframe<Color>>();
        public List<Keyframe<float>> Alphas { get; set; } = new List<Keyframe<float>>();

        public IEnumerable<Vector2> AnimatePosition(Func<float> deltaTimeFunction)
            => KeyframeAnimation.Animate(Positions, Vector2.Lerp, deltaTimeFunction, Vector2.Zero);

        public IEnumerable<float> AnimateRotation(Func<float> deltaTimeFunction)
            => KeyframeAnimation.Animate(Rotations, MathHelper.Lerp, deltaTimeFunction, 0);

        public IEnumerable<Vector2> AnimateScale(Func<float> deltaTimeFunction)
            => KeyframeAnimation.Animate(Scales, Vector2.Lerp, deltaTimeFunction, Vector2.One);

        public IEnumerable<Color> AnimateColor(Func<float> deltaTimeFunction)
            => KeyframeAnimation.Animate(Colors, Color.Lerp, deltaTimeFunction, Color.White);

        public IEnumerable<float> AnimateAlpha(Func<float> deltaTimeFunction)
            => KeyframeAnimation.Animate(Alphas, MathHelper.Lerp, deltaTimeFunction, 1);

        public void AddPositionKeyframe(float time, Vector2? value = null)
        {
            Positions.Add(new Keyframe<Vector2>(time, value ?? GetPosition(time)));
            Positions.Sort((a, b) => Comparer<float>.Default.Compare(a.Time, b.Time));
        }

        public void AddRotationKeyframe(float time, float? value = null)
        {
            Rotations.Add(new Keyframe<float>(time, value ?? GetRotation(time)));
            Rotations.Sort((a, b) => Comparer<float>.Default.Compare(a.Time, b.Time));
        }

        public void AddScaleKeyframe(float time, Vector2? value = null)
        {
            Scales.Add(new Keyframe<Vector2>(time, value ?? GetScale(time)));
            Scales.Sort((a, b) => Comparer<float>.Default.Compare(a.Time, b.Time));
        }

        public void AddColorKeyframe(float time, Color? value = null)
        {
            Colors.Add(new Keyframe<Color>(time, value ?? GetColor(time)));
            Colors.Sort((a, b) => Comparer<float>.Default.Compare(a.Time, b.Time));
        }

        public void AddAlphaKeyframe(float time, float? value = null)
        {
            Alphas.Add(new Keyframe<float>(time, value ?? GetAlpha(time)));
            Alphas.Sort((a, b) => Comparer<float>.Default.Compare(a.Time, b.Time));
        }

        public Vector2 GetPosition(float time)
            => KeyframeAnimation.GetValue(Positions, Vector2.Lerp, time, Vector2.Zero);

        public float GetRotation(float time)
            => KeyframeAnimation.GetValue(Rotations, MathHelper.Lerp, time, 0);

        public Vector2 GetScale(float time)
            => KeyframeAnimation.GetValue(Scales, Vector2.Lerp, time, Vector2.One);

        public Color GetColor(float time)
            => KeyframeAnimation.GetValue(Colors, Color.Lerp, time, Color.White);

        public float GetAlpha(float time)
            => KeyframeAnimation.GetValue(Alphas, MathHelper.Lerp, time, 1f);

        public AnimationComponent GetAnimationComponent(TimeSpan duration)
        {
            var timer = ServiceLocator.Instance.GetService<ITimerService>();
            float durationSeconds = (float)duration.TotalSeconds;
            var component = new AnimationComponent();

            var deltaTimeFunction = new Func<float>(() =>
            {
                component.CurrentTime = MathHelper.Clamp(component.CurrentTime + (float)timer.DeltaTime.TotalSeconds, 0f, 1f);
                return component.CurrentTime / durationSeconds;
            });            

            component.Position = Positions.Count > 0 ? AnimatePosition(deltaTimeFunction).GetEnumerator() : null;
            component.Rotation = Rotations.Count > 0 ? AnimateRotation(deltaTimeFunction).GetEnumerator() : null;
            component.Scale = Scales.Count > 0 ? AnimateScale(deltaTimeFunction).GetEnumerator() : null;
            component.Color = Colors.Count > 0 ? AnimateColor(deltaTimeFunction).GetEnumerator() : null;
            component.Alpha = Alphas.Count > 0 ? AnimateAlpha(deltaTimeFunction).GetEnumerator() : null;

            return component;
        }        
    }
}
