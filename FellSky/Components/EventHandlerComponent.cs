using Artemis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public class EventHandlerComponent: IComponent
    {
        public Dictionary<int, EventHandler> EventHandlers = new Dictionary<int, EventHandler>();
    }
}
