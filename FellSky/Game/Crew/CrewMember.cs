using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Crew
{
    public class CrewMember
    {
        public string NickName { get; set; }
        public string Name { get; set; }
        public List<CrewPerk> Perks { get; set; } = new List<CrewPerk>();
    }
}
