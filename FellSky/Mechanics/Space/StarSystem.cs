using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Mechanics.Space
{
    public class StarSystem
    {
        public SpaceObject MainBody { get { return Objects[0]; } }
        public List<SpaceObject> Objects { get; } = new List<SpaceObject>();
    }
}
