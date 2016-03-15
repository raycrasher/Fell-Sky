using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace FellSky.Editor
{

    public class FloatUpDownEditor : UpDownEditor<FloatUpDown, float?>
    {
    }

    
    public class FloatUpDown : CommonNumericUpDown<float>
    {        
        //FromText fromText, FromDecimal fromDecimal, Func<T, T, bool> fromLowerThan, Func<T, T, bool> fromGreaterThan
        public FloatUpDown():
            base(
                (s,st,p)=> float.Parse(s,st,p),
                (d) =>(float)d,
                (a,b) => a < b,
                (a,b) => a > b)
        {
        }
        protected override float DecrementValue(float value, float increment)
        {
            return value - increment;
        }

        protected override float IncrementValue(float value, float increment)
        {
            return value + increment;
        }

        
        
    }


}
