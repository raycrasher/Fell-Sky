using Artemis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public class HeatComponent: IComponent
    {
        public float StoredHeat { get; set; }
        public float MaxHeat { get; set; }
    }
}
