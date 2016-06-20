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

namespace FellSky.Editor
{
    /// <summary>
    /// Interaction logic for AnimationEditorControl.xaml
    /// </summary>
    public partial class AnimationEditorControl : UserControl
    {
        AnimationEditorControlViewModel Model;

        public AnimationEditorControl()
        {
            InitializeComponent();
            Model = (AnimationEditorControlViewModel)FindResource("model");
        }
    }
}
