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
        }

        public ShipEditorViewModel Model { get; private set; }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Model.Renderer.QuitFlag = true;
            Model.RendererThread.Join();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Model = (ShipEditorViewModel)FindResource("model");
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                Model.Initialize(WFHost);
            }
        }
    }
}
