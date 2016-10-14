using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FellSky.Editor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Environment.CurrentDirectory = Path.GetFullPath(FellSky.Editor.Properties.Settings.Default.DataFolder);
            base.OnStartup(e);
        }
    }
}
