using Artemis;
using FellSky.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Systems
{
    class EventSystem: Artemis.System.EntitySystem
    {
        public Dictionary<int, EventHandler> eventHandlers = new Dictionary<int, EventHandler>();

        public void FireEvent(object sender, int eventId, EventArgs args)
        {
            EventHandler handler;
            if(eventHandlers.TryGetValue(eventId, out handler))
            {
                handler?.Invoke(sender, args);
            }
        }
        
        public void RegisterListener(int eventId, EventHandler handler)
        {
            if (!eventHandlers.ContainsKey(eventId))
                eventHandlers[eventId] = null;
            eventHandlers[eventId] += handler;
        }

        public void UnregisterListener(int eventId, EventHandler handler)
        {
            if (!eventHandlers.ContainsKey(eventId))
                return;
            eventHandlers[eventId] -= handler;
        }
    }
}
