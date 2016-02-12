using Artemis.Interface;
using FellSky.Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Models.Space
{
    public class Fleet: IComponent
    {
        public string Name { get; set; } = "Unknown Fleet";
        public IList<Ship> Ships { get; set; } = new List<Ship>();
    }
}
