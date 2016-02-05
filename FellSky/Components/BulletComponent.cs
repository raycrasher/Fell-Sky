using Artemis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public class BulletComponent : IComponent
    {
        public bool IsIndiscriminate { get; internal set; }
    }
}
