using Artemis;
using FellSky.Game.Campaign.Diplomacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Combat
{
    class BattleSceneManager : Artemis.System.ProcessingSystem
    {
        readonly List<Faction> _factionsInPlay = new List<Faction>();
        readonly List<Entity> _ships = new List<Entity>();

        public override void ProcessSystem()
        {
            
        }
    }
}
