using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Models.Space
{
    public sealed class Galaxy
    {
        public List<StarSystem> StarSystems { get; } = new List<StarSystem>();
    }
}
