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
            //NavigationFrame.LoadCompleted += (o,e)=> BusyIndicator.Visibility = Visibility.Collapsed;
        }

        private void ShowShipEditorPage(object sender, RoutedEventArgs e)
        {
            ParticleEditorFrame.Visibility = Visibility.Hidden;
            ShipEditorFrame.Visibility = Visibility.Visible;
        }

        private void ShowParticleEditorPage(object sender, RoutedEventArgs e)
        {
            ShipEditorFrame.Visibility = Visibility.Hidden;
            ParticleEditorFrame.Visibility = Visibility.Visible;            
        }

        private void ShowWorldEditorPage(object sender, RoutedEventArgs e)
        {

        }
    }
}
