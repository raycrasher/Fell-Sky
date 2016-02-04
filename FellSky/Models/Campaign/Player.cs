using Artemis.Interface;
using FellSky.Campaign.Bases;
using FellSky.Campaign.Diplomacy;
using FellSky.Core.Crew;
using FellSky.Ships;
using FellSky.Space;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Core
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
