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
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
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

    }
}
