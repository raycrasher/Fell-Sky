using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Core.Crew
{

    public class CrewPerk
    {
        public class PerkRequirement
        {
            public CrewPerk Perk { get; set; }
            public int Level { get; set; }
        }

        public string Name { get; set; }
        public PerkRequirement[] Requirements { get; set; }
    }

    public static class CrewPerks
    {

    }
}
