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
        public bool IsFinished => CurrentTime >= 1f;
        public float CurrentTime;
        public IEnumerator<Vector2> Position;
        public IEnumerator<float> Rotation;
        public IEnumerator<Vector2> Scale;
        public IEnumerator<Color> Color;
        public IEnumerator<float> Alpha;
    }
}
