using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Space
{
    public sealed class StarSystem
    {
        public SpaceObject MainBody { get { return Objects[0]; } }
        public List<SpaceObject> Objects { get; } = new List<SpaceObject>();
        
        public string Name { get; set; }
    }
}
