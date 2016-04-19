using Artemis.Interface;
using FellSky.Game.Campaign.Bases;
using FellSky.Game.Campaign.Diplomacy;
using FellSky.Game.Ships;
using FellSky.Game.Space;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FellSky.Game.Crew;

namespace FellSky.Game.Campaign
{
    public class Player: IComponent
    {
        public Faction PlayerFaction { get; set; }
        public List<Ship> OwnedShips { get; set; } = new List<Ship>();
        public Fleet ControlledFleet { get; set; }
        public Ship ControlledShip { get; set; }
        public Character PlayerCharacter { get; set; }
        public Base Base { get; set; }

    }
}
