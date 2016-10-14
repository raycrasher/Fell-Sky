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
    /// Interaction logic for ParticleEditor.xaml
    /// </summary>
    public partial class ParticleEditor : Page
    {
        ParticleEditorViewModel _model;
        public ParticleEditor()
        {
            InitializeComponent();
            _model = (ParticleEditorViewModel)FindResource("model");
            
        }

        private void OnHostRender(TimeSpan obj)
        {
            _model.Render(obj);
        }

        private void OnHostInitializing()
        {
            _model.Initialize(GfxHost);
        }
    }
}
