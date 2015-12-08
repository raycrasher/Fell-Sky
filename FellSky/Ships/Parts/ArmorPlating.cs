using Artemis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Ships.Parts
{
    public class ArmorPlating: IComponent
    {
        public string IconId { get; set; }
        public float Thickness { get; set; }
        public float Resistance { get; set; }
        public float Durability { get; set; }
        public float Health { get; set; }
    }
}
