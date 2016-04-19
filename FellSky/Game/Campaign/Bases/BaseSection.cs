using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Campaign.Bases
{
    public class BaseSection
    {
        public Base Base { get; set; }
        public string Name { get; set; }
        public int Index { get; set; }
        public Point Position { get; set; }
        public BaseFacility Facility { get; set; }
    }
}
