using Artemis.Interface;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public class FadeInComponent: IComponent
    {
        public Color OriginalColor { get; set; }
        public Color CurrentColor { get; set; }
        public float AgeNormalized { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
