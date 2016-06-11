using FellSky.Game.Ships.Parts;
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
    /// Interaction logic for ShipPartEditorControl.xaml
    /// </summary>
    public partial class ShipPartEditorControl : UserControl
    {



        public List<ShipPart> Parts
        {
            get { return (List<ShipPart>)GetValue(PartsProperty); }
            set { SetValue(PartsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Parts.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PartsProperty =
            DependencyProperty.Register("Parts", typeof(List<ShipPart>), typeof(ShipPartEditorControl), new PropertyMetadata(OnPartsChanged));


        private static void OnPartsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ShipPartEditorControl)d)._model.Parts = (List<ShipPart>)e.NewValue;
        }

        private ShipPartEditorViewModel _model;

        public ShipPartEditorControl()
        {
            InitializeComponent();
            _model = (ShipPartEditorViewModel) FindResource("Model");
            _model.Parts = Parts;
        }
    }
}
