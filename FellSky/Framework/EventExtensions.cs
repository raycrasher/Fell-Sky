using Artemis;
using FellSky.Components;
using FellSky.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky
{
    public static class EventExtensions
    {
        public static void RegisterListener(this EntityWorld world, int eventId, EventHandler handler)
        {
            world.SystemManager.GetSystem<EventSystem>().RegisterListener(eventId, handler);
        }

        public static void RegisterEvent(this Entity entity, int eventId, EventHandler handler)
        {
            var component = entity.GetComponent<EventHandlerComponent>();
            if (component == null)
            {
                component = new EventHandlerComponent();
                entity.AddComponent(component);
            }
            if (!component.EventHandlers.ContainsKey(eventId))
                component.EventHandlers[eventId] = null;
            component.EventHandlers[eventId] += handler;
        }

        public static void UnregisterListener(this EntityWorld world, int eventId, EventHandler handler)
        {
            world.SystemManager.GetSystem<EventSystem>().UnregisterListener(eventId, handler);
        }

        public static void UnregisterListener(this Entity entity, int eventId, EventHandler handler)
        {
            var component = entity.GetComponent<EventHandlerComponent>();
            if (component == null)
                return;
        }

        public static void FireEvent(this Entity entity, object sender, int eventId, EventArgs args)
        {
            var component = entity.GetComponent<EventHandlerComponent>();
            if (component == null)
                return;
            EventHandler handler;
            if(component.EventHandlers.TryGetValue(eventId, out handler))
            {
                handler?.Invoke(sender, args);
            }
        }

        public static void FireEvent(this EntityWorld world, object sender, int eventId, EventArgs args)
        {
            world.SystemManager.GetSystem<EventSystem>().FireEvent(sender, eventId, args);
        }
    }
}
