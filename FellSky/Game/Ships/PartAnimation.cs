using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Ships
{
    public class Keyframe<T>
    {
        public Keyframe(float time, T value)
        {
            Time = time;
            Value = value;
        }

        public Keyframe() { }
        public float Time { get; set; }
        
        /// <summary>
        /// The value, relative to the previous keyframe.
        /// </summary>
        public T Value { get; set; }
    }

    public class PartAnimation
    {
        public delegate T LerpFunction<T>(T start, T end, float percentage);

        public List<Keyframe<Vector2>> Positions { get; set; } = new List<Keyframe<Vector2>>();
        public List<Keyframe<float>> Rotations { get; set; } = new List<Keyframe<float>>();
        public List<Keyframe<Vector2>> Scales { get; set; } = new List<Keyframe<Vector2>>();
        public List<Keyframe<Color>> Colors { get; set; } = new List<Keyframe<Color>>();
        public List<Keyframe<float>> Alphas { get; set; } = new List<Keyframe<float>>();

        public IEnumerable<Vector2> AnimatePosition(Func<float> deltaTimeFunction)
            => Animate(Positions, Vector2.Lerp, deltaTimeFunction, Vector2.Zero);

        public IEnumerable<float> AnimateRotation(Func<float> deltaTimeFunction)
            => Animate(Rotations, MathHelper.Lerp, deltaTimeFunction, 0);

        public IEnumerable<Vector2> AnimateScale(Func<float> deltaTimeFunction)
            => Animate(Scales, Vector2.Lerp, deltaTimeFunction, Vector2.One);

        public IEnumerable<Color> AnimateColor(Func<float> deltaTimeFunction)
            => Animate(Colors, Color.Lerp, deltaTimeFunction, Color.White);

        public IEnumerable<float> AnimateAlpha(Func<float> deltaTimeFunction)
            => Animate(Alphas, MathHelper.Lerp, deltaTimeFunction, 1);

        public void Sort()
        {
            Positions.Sort((a, b) => Comparer<float>.Default.Compare(a.Time, b.Time));
            Rotations.Sort((a, b) => Comparer<float>.Default.Compare(a.Time, b.Time));
            Scales.Sort((a, b) => Comparer<float>.Default.Compare(a.Time, b.Time));
            Colors.Sort((a, b) => Comparer<float>.Default.Compare(a.Time, b.Time));
            Alphas.Sort((a, b) => Comparer<float>.Default.Compare(a.Time, b.Time));
        }

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
            => GetValue(Positions, Vector2.Lerp, time, Vector2.Zero);

        public float GetRotation(float time)
            => GetValue(Rotations, MathHelper.Lerp, time, 0);

        public Vector2 GetScale(float time)
            => GetValue(Scales, Vector2.Lerp, time, Vector2.One);

        public Color GetColor(float time)
            => GetValue(Colors, Color.Lerp, time, Color.White);

        public float GetAlpha(float time)
            => GetValue(Alphas, MathHelper.Lerp, time, 1f);


        public static IEnumerable<T> Animate<T>(List<Keyframe<T>> frames, LerpFunction<T> lerpFunction, Func<float> deltaTimeFunction, T defaultValue)
        {
            float time = 0;
            float end = frames.Count <= 0 ? 0 : frames[frames.Count - 1].Time;

            // no keyframe case
            if (frames.Count <= 0)
            {
                yield return defaultValue;
                yield break;
            }

            // one keyframe case
            if (frames.Count == 1)
            {
                var keyframe = frames[0];
                while (time < end)
                {
                    yield return lerpFunction(defaultValue, keyframe.Value, time / keyframe.Time);
                    time += deltaTimeFunction();
                }
                yield return keyframe.Value;
                yield break;
            }

            // many keyframes case
            int index = 0;
            Keyframe<T> currentFrame = frames[index];
            T lastValue = defaultValue;
            float lastTime = 0f;

            while (index < frames.Count - 1)
            {
                yield return lerpFunction(lastValue, currentFrame.Value, (time - lastTime) / currentFrame.Time);
                time += deltaTimeFunction();

                if (time > frames[index + 1].Time)
                {
                    lastValue = currentFrame.Value;
                    lastTime = currentFrame.Time;
                    index++;
                    currentFrame = frames[index];
                }
            }
            yield return currentFrame.Value;
        }

        public static T GetValue<T>(List<Keyframe<T>> frames, LerpFunction<T> lerpFunction, float time, T defaultValue)
        {
            if (frames.Count <= 0) return defaultValue;

            int i;
            Keyframe<T> last = null, current = null;

            for (i = 0; i < frames.Count; i++)
            {
                last = current;
                current = frames[i];
                if (frames[i].Time < time) break;
            }

            if (i >= frames.Count)
                return current.Value;
            else if (last == null)
            {
                return lerpFunction(defaultValue, current.Value, time / current.Time);
            }
            else
            {
                return lerpFunction(last.Value, current.Value, time / current.Time);
            }
        }
    }
}
