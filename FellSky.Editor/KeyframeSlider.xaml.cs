using FellSky.Framework;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using XnaColor = Microsoft.Xna.Framework.Color;

namespace FellSky.Editor
{
    /// <summary>
    /// Interaction logic for KeyframeSlider.xaml
    /// </summary>
    public partial class KeyframeSlider : UserControl
    {
        public System.Collections.IList Frames
        {
            get { return (System.Collections.IList)GetValue(FramesProperty); }
            set { SetValue(FramesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Frames.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FramesProperty =
            DependencyProperty.Register("Frames", typeof(System.Collections.IList), typeof(KeyframeSlider), new PropertyMetadata());



        public object DefaultValue
        {
            get { return (object)GetValue(DefaultValueProperty); }
            set { SetValue(DefaultValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DefaultValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DefaultValueProperty =
            DependencyProperty.Register("DefaultValue", typeof(object), typeof(KeyframeSlider), new PropertyMetadata());



        public IKeyframe CurrentKeyframe
        {
            get { return (IKeyframe)GetValue(CurrentKeyframeProperty); }
            set { SetValue(CurrentKeyframeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentKeyframe.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentKeyframeProperty =
            DependencyProperty.Register("CurrentKeyframe", typeof(IKeyframe), typeof(KeyframeSlider), new PropertyMetadata());

        private IKeyframe _draggedKeyframe;

        public KeyframeSlider()
        {
            InitializeComponent();
        }

        private void OnSliderMouseDown(object sender, MouseButtonEventArgs e)
        {
            var canvas = (ItemsControl)sender;
            var pos = e.GetPosition(canvas);
            var percentage = (float)(pos.X / canvas.ActualWidth);

            if (Frames is List<Keyframe<float>>)
            {
                var list = (List<Keyframe<float>>)Frames;
                list.Add(new Keyframe<float>(percentage, KeyframeAnimation.GetValue(list, MathHelper.Lerp, percentage, (float)DefaultValue)));
                CurrentKeyframe = list[list.Count - 1];
                list.Sort(Keyframe<float>.Compare);
            }
            else if (Frames is List<Keyframe<Vector2>>)
            {
                var list = (List<Keyframe<Vector2>>)Frames;
                list.Add(new Keyframe<Vector2>(percentage, KeyframeAnimation.GetValue(list, Vector2.Lerp, percentage, (Vector2)DefaultValue)));
                CurrentKeyframe = list[list.Count - 1];
                list.Sort(Keyframe<Vector2>.Compare);
            }
            else if (Frames is List<Keyframe<XnaColor>>)
            {
                var list = (List<Keyframe<XnaColor>>)Frames;
                list.Add(new Keyframe<XnaColor>(percentage, KeyframeAnimation.GetValue(list, XnaColor.Lerp, percentage, (XnaColor)DefaultValue)));
                CurrentKeyframe = list[list.Count - 1];
                list.Sort(Keyframe<XnaColor>.Compare);
            }
            else throw new InvalidOperationException("Keyframe type not supported.");

            CollectionViewSource.GetDefaultView(Frames).Refresh();
        }

        private void OnKeyframeMouseDown(object sender, MouseButtonEventArgs e)
        {
            _draggedKeyframe = (IKeyframe)((System.Windows.Shapes.Rectangle)sender).Tag;
            CurrentKeyframe = _draggedKeyframe;
            slider.Cursor = Cursors.SizeWE;
            e.Handled = true;
        }

        private void OnDragKeyframe(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && _draggedKeyframe != null)
            {
                var canvas = (ItemsControl)sender;
                var pos = e.GetPosition(canvas);
                var percentage = pos.X / canvas.ActualWidth;

                _draggedKeyframe.Time = (float)percentage;
                CollectionViewSource.GetDefaultView(Frames).Refresh();
            }
        }

        private void SortFrames()
        {
            if (Frames is List<Keyframe<float>>)
            {
                var list = (List<Keyframe<float>>)Frames;
                list.Sort(Keyframe<float>.Compare);
            }
            else if (Frames is List<Keyframe<Vector2>>)
            {
                var list = (List<Keyframe<Vector2>>)Frames;
                list.Sort(Keyframe<Vector2>.Compare);
            }
            else if (Frames is List<Keyframe<XnaColor>>)
            {
                var list = (List<Keyframe<XnaColor>>)Frames;
                list.Sort(Keyframe<XnaColor>.Compare);
            }
            else throw new InvalidOperationException("Keyframe type not supported.");
        }

        private void Reset(object sender, EventArgs e)
        {
            slider.Cursor = Cursors.Hand;
            if(_draggedKeyframe!=null)
                SortFrames();
            _draggedKeyframe = null;

        }
    }
}
