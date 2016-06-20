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
    public class AnimationEditorControlViewModel: INotifyPropertyChanged
    {
        public PartAnimation Animation { get; set; } = new PartAnimation();
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }
        public float Rotation { get; set; }
        public float Alpha { get; set; }
        public System.Windows.Media.Color? Color { get; set; }

        public float CurrentTime { get; set; }

        public IKeyframe PositionKeyframe { get; set; }
        public int CurrentPositionYIndex { get; set; }
        public int CurrentRotIndex { get; set; }
        public int CurrentScaleXIndex { get; set; }
        public int CurrentScaleYIndex { get; set; }
        public int CurrentColorIndex { get; set; }
        public int CurrentAlphaIndex { get; set; }

        public Vector2 DefaultPosition { get; set; } = Vector2.Zero;
        public float DefaultRotation { get; set; } = 0f;
        public Vector2 DefaultScale { get; set; } = Vector2.One;
        public Color DefaultColor { get; set; } = Microsoft.Xna.Framework.Color.Black;
        public float DefaultAlpha { get; set; } = 0f;
        

        public AnimationEditorControlViewModel()
        {
            Animation.Positions = new[] { new Keyframe<Vector2>(0.4f, Vector2.One), new Keyframe<Vector2>(0.8f, new Vector2(10, -40)) }.ToList();
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(PositionKeyframe))
            {
                var k = (Vector2)PositionKeyframe.Value;
                PositionX = k.X;
                PositionY = k.Y;
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
