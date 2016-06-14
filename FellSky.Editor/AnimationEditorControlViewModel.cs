using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Editor
{
    [PropertyChanged.ImplementPropertyChanged]
    public class AnimationEditorControlViewModel
    {
        public float? PositionX { get; set; }
        public float? PositionY { get; set; }
        public float? ScaleX { get; set; }
        public float? ScaleY { get; set; }
        public float? Rotation { get; set; }
        public float? Alpha { get; set; }
        public System.Windows.Media.Color? Color { get; set; }

        public float CurrentTime { get; set; }

        public int CurrentPositionXIndex { get; set; }
        public int CurrentPositionYIndex { get; set; }
        public int CurrentRotIndex { get; set; }
        public int CurrentScaleXIndex { get; set; }
        public int CurrentScaleYIndex { get; set; }
        public int CurrentColorIndex { get; set; }
        public int CurrentAlphaIndex { get; set; }

        internal void AddPositionAt(double percentage)
        {
            throw new NotImplementedException();
        }
    }
}
