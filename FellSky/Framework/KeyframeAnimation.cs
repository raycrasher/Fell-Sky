using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Framework
{
    public delegate T LerpFunction<T>(T start, T end, float percentage);

    public interface IKeyframe
    {
        float Time { get; set; }
        object Value { get; }
    }

    public sealed class Keyframe<T>: IKeyframe
    {
        public Keyframe(float time, T value)
        {
            Time = time;
            Value = value;
        }

        public Keyframe() { }
        public float Time { get; set; }

        object IKeyframe.Value
        {
            get { return Value; }
        }

        /// <summary>
        /// The value, relative to the previous keyframe.
        /// </summary>
        public T Value { get; set; }

        public static int Compare(Keyframe<T> a, Keyframe<T> b)
            => Math.Sign(a.Time - b.Time);

        public override string ToString()
        {
            return $"T+{Time}   {Value}";
        }
    }

    public static class KeyframeAnimation
    {
        public static IEnumerable<T> Animate<T>(IList<Keyframe<T>> frames, LerpFunction<T> lerpFunction, Func<float> deltaTimeFunction, T defaultValue)
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

        public static T GetValue<T>(IList<Keyframe<T>> frames, LerpFunction<T> lerpFunction, float time, T defaultValue)
        {
            if (frames.Count <= 0) return defaultValue;

            int i;
            Keyframe<T> last = null, current = null;

            for (i = 0; i < frames.Count; i++)
            {
                last = current;
                current = frames[i];
                if (frames[i].Time > time) break;
            }

            if (i >= frames.Count)
                return current.Value;
            else if (last == null)
            {
                return lerpFunction(defaultValue, current.Value, time / current.Time);
            }
            else
            {
                return lerpFunction(last.Value, current.Value, (time - last.Time) / current.Time);
            }
        }
    }
}
