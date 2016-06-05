using Artemis.Interface;
using FellSky.Game.Ships.Parts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;

namespace FellSky.Components
{
    public class HardpointComponent: IComponent
    {
        public HardpointComponent(Hardpoint hardpoint)
        {
            Hardpoint = hardpoint;
        }

        public Hardpoint Hardpoint;
        public Entity InstalledEntity;
    }
}
