using FellSky.Framework;
using FellSky.Game.Ships;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FellSky.Editor
{
    [PropertyChanged.ImplementPropertyChanged]
    public class AnimationEditorControlViewModel
    {
        public PartAnimation Animation { get; set; } = new PartAnimation();

        public float CurrentTime { get; set; }

        public KeyframeVector2ViewModel PositionKeyframe { get; } = new KeyframeVector2ViewModel();
        public KeyframeFloatViewModel RotationKeyframe { get; } = new KeyframeFloatViewModel();
        public KeyframeVector2ViewModel ScaleKeyframe { get; } = new KeyframeVector2ViewModel();
        public KeyframeColorViewModel ColorKeyframe { get; } = new KeyframeColorViewModel();
        public KeyframeFloatViewModel AlphaKeyframe { get; } = new KeyframeFloatViewModel();

        public Vector2 DefaultPosition { get; set; } = Vector2.Zero;
        public float DefaultRotation { get; set; } = 0f;
        public Vector2 DefaultScale { get; set; } = Vector2.One;
        public Color DefaultColor { get; set; } = Microsoft.Xna.Framework.Color.Black;
        public float DefaultAlpha { get; set; } = 0f;
    }

    [PropertyChanged.ImplementPropertyChanged]
    public class KeyframeVector2ViewModel
    {
        public float? X
        {
            get { return ((Keyframe<Vector2>)Keyframe)?.Value.X; }
            set
            {
                if (value != null && Keyframe != null)
                    ((Keyframe < Vector2 > )Keyframe).Value = new Vector2((float)value, ((Keyframe<Vector2>)Keyframe).Value.Y);
            }
        }
        public float? Y
        {
            get { return ((Keyframe<Vector2>)Keyframe)?.Value.Y; }
            set
            {
                if (value != null && Keyframe != null)
                    ((Keyframe < Vector2 > )Keyframe).Value = new Vector2(((Keyframe<Vector2>)Keyframe).Value.X, (float)value);
            }
        }
        [PropertyChanged.AlsoNotifyFor("X","Y")]
        public IKeyframe Keyframe {
            get;
            set;
        }
    }

    [PropertyChanged.ImplementPropertyChanged]
    public class KeyframeFloatViewModel
    {
        public float? Value
        {
            get { return ((Keyframe<float>)Keyframe)?.Value; }
            set
            {
                if (value != null && Keyframe != null)
                    ((Keyframe<float>)Keyframe).Value = (float) value;
            }
        }
        [PropertyChanged.AlsoNotifyFor("Value")]
        public IKeyframe Keyframe
        {
            get;
            set;
        }
    }

    [PropertyChanged.ImplementPropertyChanged]
    public class KeyframeColorViewModel
    {
        private Keyframe<Color> ColorKeyframe => (Keyframe<Color>)Keyframe;
        public System.Windows.Media.Color? Color
        {
            get {
                if (ColorKeyframe == null) return null;
                return System.Windows.Media.Color.FromArgb(ColorKeyframe.Value.A, ColorKeyframe.Value.R,ColorKeyframe.Value.G,ColorKeyframe.Value.B);
            }
            set
            {
                if (value != null && Keyframe != null)
                {
                    ColorKeyframe.Value = value.Value.ToXnaColor();
                }
            }
        }
        
        [PropertyChanged.AlsoNotifyFor("Color")]
        public IKeyframe Keyframe
        {
            get;
            set;
        }
    }


}
