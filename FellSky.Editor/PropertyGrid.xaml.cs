using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Forms;
using System;

namespace FellSky.Editor
{
    /// <summary>
    /// Interaction logic for PropertyGrid.xaml
    /// </summary>
    public partial class PropertyGrid : System.Windows.Controls.UserControl
    {
        public object PropertyObject
        {
            get { return (object)GetValue(PropertyObjectProperty); }
            set
            {
                if(value is Array)
                {
                    WinFormsPropertyGrid.SelectedObject = null;
                    WinFormsPropertyGrid.SelectedObjects = (object[])value;
                } else
                {
                    WinFormsPropertyGrid.SelectedObjects = null;
                    WinFormsPropertyGrid.SelectedObject = value;
                }

                SetValue(PropertyObjectProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for PropertyObject.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PropertyObjectProperty =
            DependencyProperty.Register("PropertyObject", typeof(object), typeof(PropertyGrid), new PropertyMetadata(PropertyObjectChanged));
        private System.Windows.Forms.PropertySort _lastSort;

        private static void PropertyObjectChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var pg = o as PropertyGrid;
            if (pg != null)
                pg.WinFormsPropertyGrid.SelectedObject = e.NewValue;
        }

        public PropertyGrid()
        {
            InitializeComponent();

            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                WinFormsPropertyGrid.Visible = false;
            } else WinFormsPropertyGrid.Visible = true;

            Loaded += (o, e) =>
            {
                WinFormsPropertyGrid.SelectedObject = PropertyObject;
            };
            
            WinFormsPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            _lastSort = WinFormsPropertyGrid.PropertySort;

            var toolstrip = WinFormsPropertyGrid.Controls.OfType<ToolStrip>().First();
            var categoryButton = toolstrip.Items[0] as ToolStripButton;
            var alphaButton = toolstrip.Items[1] as ToolStripButton;
            categoryButton.Click += (o, e) =>
            {
                if (WinFormsPropertyGrid.PropertySort != PropertySort.NoSort && (_lastSort == PropertySort.Categorized || _lastSort == PropertySort.CategorizedAlphabetical))
                {
                    WinFormsPropertyGrid.PropertySort = PropertySort.NoSort;
                }
                else
                {
                    WinFormsPropertyGrid.PropertySort = PropertySort.Categorized;
                }
               _lastSort = WinFormsPropertyGrid.PropertySort;
            };

            alphaButton.Click += (o, e) =>
            {
                if (WinFormsPropertyGrid.PropertySort != PropertySort.NoSort && _lastSort == PropertySort.Alphabetical)
                {
                    WinFormsPropertyGrid.PropertySort = PropertySort.NoSort;
                }
                _lastSort = WinFormsPropertyGrid.PropertySort;
            };

            WinFormsPropertyGrid.PropertySortChanged += (o, e) =>
            {
                if (WinFormsPropertyGrid.PropertySort == PropertySort.CategorizedAlphabetical)
                    WinFormsPropertyGrid.PropertySort = PropertySort.Categorized;
            };
            

            //WinFormsPropertyGrid.ToolbarVisible = false;
        }
    }
}