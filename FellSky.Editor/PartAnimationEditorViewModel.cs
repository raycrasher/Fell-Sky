using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Editor
{
    [PropertyChanged.ImplementPropertyChanged]
    public class PartAnimationEditorViewModel
    {
        public float? PositionX { get; set; }
        public float? PositionY { get; set; }
        public float? ScaleX { get; set; }
        public float? ScaleY { get; set; }
        public float? Rotation { get; set; }

        public float CurrentTime { get; set; }
    }
}
