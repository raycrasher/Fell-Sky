using FellSky.Game.Ships;
using FellSky.Game.Space;
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

        public Galaxy Galaxy { get; set; }
        public SpaceObject CurrentOrbitedBody { get; set; }

        public List<Ship> AllShips { get; set; } = new List<Ship>();
        public CampaignSettings Settings { get; private set; }

        public Campaign(CampaignSettings settings)
        {
            Settings = settings;
        }

        

        public void UpdateCampaign()
        {

        }
    }
}
