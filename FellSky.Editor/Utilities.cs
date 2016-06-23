using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Editor
{
    static class Utilities
    {
        public static Microsoft.Xna.Framework.Color ToXnaColor(this System.Drawing.Color c)
        {
            return new Microsoft.Xna.Framework.Color(c.R, c.G, c.B, c.A);
        }

        public static Microsoft.Xna.Framework.Color ToXnaColor(this System.Windows.Media.Color c)
        {
            return new Microsoft.Xna.Framework.Color(c.R, c.G, c.B, c.A);
        }
    }
}
