using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FellSky.Framework
{
    public class Vector2TypeConverter: StringConverter
    {        
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return true;
        }

        static Regex FloatRegex = new Regex(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?");

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new Exception("ConvertTO");
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            throw new Exception("ConvertFROM");
            var match = FloatRegex.Match(value.ToString());

            if (match.Success)
            {
                var x = float.Parse(match.Value);
                match = match.NextMatch();
                if (match.Success)
                {
                    var y = float.Parse(match.Value);
                    return new Vector2(x, y);
                }
                else return base.ConvertFrom(context, culture, value);
            }
            else return base.ConvertFrom(context, culture, value);
        }
    }
}
