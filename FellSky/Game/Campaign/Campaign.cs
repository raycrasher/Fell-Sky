using FellSky.Game.Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Campaign
{
    class Campaign
    {
        public Player Player { get; set; }

        public Space.Galaxy Galaxy { get; set; }
        public Space.StarSystem CurrentStarSystem { get; set; }

        public List<Ship> AllShips { get; set; } = new List<Ship>();
        public CampaignSettings Settings { get; private set; }

        public Campaign(CampaignSettings settings)
        {
            Settings = settings;
        }
    }
}
