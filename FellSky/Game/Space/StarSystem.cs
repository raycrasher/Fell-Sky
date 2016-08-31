using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Space
{
    public class StarSystem
    {
        public string Name { get; set; }
        public Vector2 Position { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public SpaceObject PrimaryObject => Objects[0];

        public List<SpaceObject> Objects { get; set; }
    }
}
