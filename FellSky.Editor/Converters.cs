
using System;
using System.Globalization;
using XnaColor = Microsoft.Xna.Framework.Color;
using WpfColor = System.Windows.Media.Color;
using FellSky.Framework;

namespace FellSky.Editor
{
    public class SpriteToIntRectConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var sprite = value as Sprite;
            if (sprite == null) return null;
            return new System.Windows.Int32Rect(sprite.X, sprite.Y, sprite.W, sprite.H);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class XnaColorToBrushConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is XnaColor)) return null;
            var color = (XnaColor)value;
            return new System.Windows.Media.SolidColorBrush(WpfColor.FromArgb(color.A, color.R, color.G, color.B));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brush = value as System.Windows.Media.SolidColorBrush;
            if (brush == null) return null;
            return new XnaColor(brush.Color.R, brush.Color.G, brush.Color.B, brush.Color.A);
        }
    }

    public class XnaColorToWindowsColorConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is XnaColor)
            {
                var c = (XnaColor)value;
                return WpfColor.FromArgb(c.A, c.R, c.G, c.B);
            }
            else if(value is WpfColor)
            {
                var c = (WpfColor)value;
                return new XnaColor(c.R, c.G, c.B, c.A);
            }
            throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }

    public class ContrastingColorConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is System.Windows.Media.SolidColorBrush))
                return System.Windows.Media.Brushes.Black;
            WpfColor color = ((System.Windows.Media.SolidColorBrush)value).Color;
            double Y = 0.2126 * color.ScR + 0.7152 * color.ScG + 0.0722 * color.ScB;
            return Y > 0.2 ? System.Windows.Media.Brushes.Black : System.Windows.Media.Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class EnumToBooleanConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString() == parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value.Equals(true))
            {
                return Enum.Parse(targetType, parameter.ToString());
            }
            return null;
        }
    }

    public class UpDownFloatConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (targetType == typeof(float?)) return (float?)(float)value;
            if (targetType == typeof(decimal?)) return (decimal?)(float)value;
            //if (targetType == typeof(double?)) return (double?)(float)value;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (float)(decimal)value;
        }
    }

    public class EnumValuesConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Enum.GetValues((Type)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
