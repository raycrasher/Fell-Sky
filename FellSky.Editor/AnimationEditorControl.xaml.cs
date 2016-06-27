using FellSky.Framework;
using FellSky.Game.Ships;
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

namespace FellSky.Editor
{
    /// <summary>
    /// Interaction logic for AnimationEditorControl.xaml
    /// </summary>
    public partial class AnimationEditorControl : UserControl
    {
        public PartAnimation Animation
        {
            get { return (PartAnimation)GetValue(AnimationProperty); }
            set { SetValue(AnimationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Animation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnimationProperty =
            DependencyProperty.Register("Animation", typeof(PartAnimation), typeof(AnimationEditorControl), new PropertyMetadata(OnAnimationChanged));
        
        AnimationEditorControlViewModel Model;
        
        public AnimationEditorControl()
        {
            InitializeComponent();
            Model = (AnimationEditorControlViewModel)FindResource("model");
        }

        static void OnAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctl = (AnimationEditorControl)d;
            ctl.Model.Animation = (PartAnimation)e.NewValue;
        }
    }
}
