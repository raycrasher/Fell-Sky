using Artemis.Interface;
using FellSky.Game.Ships.Parts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public class DoodadComponent: IComponent
    {
        public DoodadComponent(Doodad doodad)
        {
            Doodad = doodad;
        }

        public DoodadComponent() { }

        public Doodad Doodad;
    }
}
