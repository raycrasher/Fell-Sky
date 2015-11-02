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
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Model = (ShipEditorViewModel)FindResource("model");
        }

        public ShipEditorViewModel Model { get; private set; }

        private void D3D11Host_Initializing()
        {
            Model.Initialize(D3D11Host);
        }

        private void D3D11Host_Rendering(TimeSpan timespan)
        {
            Model.Render(timespan);
        }
    }
}
