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
        public T Value { get; set; }
    }

    public class PartAnimation
    {
        public List<Keyframe<Vector2>> Position { get; set; } = new List<Keyframe<Vector2>>();
        public List<Keyframe<float>> Rotation { get; set; } = new List<Keyframe<float>>();
        public List<Keyframe<Vector2>> Scale { get; set; } = new List<Keyframe<Vector2>>();
        public List<Keyframe<Color>> Color { get; set; } = new List<Keyframe<Color>>();
        public List<Keyframe<float>> Alpha { get; set; } = new List<Keyframe<float>>();

        public void Sort()
        {
            Position.Sort((a, b) => Comparer<float>.Default.Compare(a.Time, b.Time));
            Rotation.Sort((a, b) => Comparer<float>.Default.Compare(a.Time, b.Time));
            Scale.Sort((a, b) => Comparer<float>.Default.Compare(a.Time, b.Time));
            Color.Sort((a, b) => Comparer<float>.Default.Compare(a.Time, b.Time));
            Alpha.Sort((a, b) => Comparer<float>.Default.Compare(a.Time, b.Time));
        }

        public void AddPositionKeyframe(float percentage)
        {
            Position.Add(new Keyframe<Vector2>(percentage, GetPosition(percentage)));
            Position.Sort((a, b) => Comparer<float>.Default.Compare(a.Time, b.Time));
        }

        private Vector2 GetPosition(Vector2 initial, float percentage)
        {
            foreach(var item in Position)
            {

            }
        }

        private int GetLastIndex<T>(List<Keyframe<T>> list, float percentage)
        {
            int index;

            for(index = 0; index < list.Count; index++)
            {

            }
            return index;
        }
    }
}
