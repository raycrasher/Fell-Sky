using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Campaign.Diplomacy
{
    public class Reputation
    {
        public Dictionary<Faction, float> PercievedRelations { get; set; } = new Dictionary<Faction, float>();
        public float Alignment { get; set; }
        public float Reliability { get; set; }
        public float Loyalty { get; set; }
        public float Integrity { get; set; }
    }
}
