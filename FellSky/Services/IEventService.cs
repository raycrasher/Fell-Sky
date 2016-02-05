using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FellSky.Systems;
using System.Reflection;

namespace FellSky.Services
{
    public static partial class EventId
    {
        static EventId()
        {
            int counter = 1;
            foreach(var field in typeof(EventId).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                field.SetValue(null, counter);
                counter++;
            }
        }
    }

    public interface IEventService
    {
        void AddEventListener(int id, EventHandler handler);
        void FireEvent(object sender, int id, EventArgs eventArgs);
        void RemoveEventListener(int id, EventHandler handler);
    }
}
