using Artemis.Interface;
using FellSky.Game.Space;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public class SpaceObjectComponent: IComponent
    {
        public SpaceObject Object { get; private set; }

        public SpaceObjectComponent(SpaceObject obj)
        {
            Object = obj;
        }
    }
}
