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
    public class AnimationEditorControlViewModel
    {
        public PartAnimation Animation { get; set; } = new PartAnimation();

        public float CurrentTime { get; set; }

        public KeyframeVector2ViewModel PositionKeyframe { get; } = new KeyframeVector2ViewModel();
        public Keyframe<float> RotationKeyframe { get; }
        public Keyframe<Vector2> ScaleKeyframe { get; set; }
        public Keyframe<Color> ColorKeyframe { get; set; }
        public Keyframe<float> AlphaKeyframe { get; set; }

        public Vector2 DefaultPosition { get; set; } = Vector2.Zero;
        public float DefaultRotation { get; set; } = 0f;
        public Vector2 DefaultScale { get; set; } = Vector2.One;
        public Color DefaultColor { get; set; } = Microsoft.Xna.Framework.Color.Black;
        public float DefaultAlpha { get; set; } = 0f;
        

        public AnimationEditorControlViewModel()
        {
            Animation.Positions = new[] { new Keyframe<Vector2>(0.4f, Vector2.One), new Keyframe<Vector2>(0.8f, new Vector2(10, -40)) }.ToList();
        }
    }
}
