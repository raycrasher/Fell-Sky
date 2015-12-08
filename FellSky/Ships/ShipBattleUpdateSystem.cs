using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using FellSky.Ships;

namespace FellSky.Ships
{
    public class ShipBattleUpdateSystem : Artemis.System.EntityComponentProcessingSystem<Ship>
    {
        public override void Process(Entity entity, Ship ship)
        {
            UpdateHealth(entity, ship);
            UpdateHeat(entity, ship);
            UpdatePower(entity, ship);
        }

        private void UpdatePower(Entity entity, Ship ship)
        {
            
        }

        private void UpdateHeat(Entity entity, Ship ship)
        {
            
        }

        private void UpdateHealth(Entity entity, Ship ship)
        {
            
        }
    }
}
