using MahApps.Metro.Controls;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ShipEditorWindow : MetroWindow
    {
        public ShipEditorWindow()
        {
            InitializeComponent();
          
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                return;
            Model = (ShipEditorViewModel)FindResource("model");
        }

        public ShipEditorViewModel Model { get; private set; }

        private void D3D11Host_Initializing()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                return;
            Model?.Initialize(D3D11Host);
        }

        private void D3D11Host_Rendering(TimeSpan timespan)
        {
            Model?.Render(timespan);
        }

        private void D3D11Host_MouseEnter(object sender, MouseEventArgs e)
        {
            D3D11Host.Focus();
        }

        private void OpenColorPicker(object sender, MouseButtonEventArgs e)
        {
            var button = (RadioButton)sender;
            ColorPicker.IsOpen = true;
            ColorPicker.Tag = button;
            ColorCanvas.SelectedColor = ((SolidColorBrush)button.Background).Color;
        }

        private void PickerColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            var button = ColorPicker.Tag as RadioButton;
            if (button == null) return;
            button.SetValue(BackgroundProperty, new SolidColorBrush(e.NewValue.Value));
            BindingOperations.GetBindingExpression(button, BackgroundProperty).UpdateSource();
        }

        private void D3D11Host_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ColorPicker.IsOpen = false;
        }
    }
}
