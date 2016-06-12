using Artemis;
using FellSky.Game.Ships.Parts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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



        public ObservableCollection<Entity> Parts
        {
            get { return (ObservableCollection<Entity>)GetValue(PartsProperty); }
            set { SetValue(PartsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Parts.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PartsProperty =
            DependencyProperty.Register("Parts", typeof(ObservableCollection<Entity>), typeof(ShipPartEditorControl), new PropertyMetadata(OnPartsChanged));


        private static void OnPartsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ShipPartEditorControl)d)._model.Parts = (ObservableCollection<Entity>)e.NewValue;
        }

        private ShipPartEditorViewModel _model;

        public ShipPartEditorControl()
        {
            InitializeComponent();
            _model = (ShipPartEditorViewModel) FindResource("Model");
            
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                _model.Parts = Parts;
                _model.HasItems = true;
                _model.ShowHardpointPanel = true;
                _model.ShowHullPanel = true;
                _model.ShowThrusterPanel = true;
            }
        }
    }
}
