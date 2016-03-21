using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FellSky.Systems;
using System.Reflection;
using Microsoft.Xna.Framework;
using Priority_Queue;

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
        void FireEvent(object sender, int id, EventArgs args);
        void FireEventNextFrame(object sender, int id, EventArgs args);
        void FireEventAfter(object sender, int id, EventArgs args, TimeSpan time);
        void RemoveEventListener(int id, EventHandler handler);
        void Update(GameTime gameTime);
        void CancelAllTimedEvents();
        void CancelAllNextFrameEvents();
    }

    public class EventService : IEventService
    {
        private TimeSpan _elapsedTime = TimeSpan.Zero;

        class QueuedEvent
        {
            public object Sender;
            public int Id;
            public EventArgs Args;
            public QueuedEvent(object obj, int id, EventArgs args)
            {
                Sender = obj;
                Id = id;
                Args = args;
            }
        }

        class TimedQueuedEvent: QueuedEvent
        {
            public TimedQueuedEvent(object obj, int id, EventArgs args, TimeSpan time)
                : base(obj,id,args)
            {
                Time = time;
            }

            public TimeSpan Time { get; set; }
        }

        private readonly SimplePriorityQueue<TimedQueuedEvent> _timedEvents = new SimplePriorityQueue<TimedQueuedEvent>();
        private readonly Dictionary<int, EventHandler> _handlers = new Dictionary<int, EventHandler>();
        private readonly Queue<QueuedEvent> _nextFrameEvents = new Queue<QueuedEvent>();
        
        public void AddEventListener(int id, EventHandler handler)
        {
            _handlers[id] += handler;
        }

        public void FireEvent(object sender, int id, EventArgs args)
        {
            EventHandler handler;
            if(_handlers.TryGetValue(id, out handler))
            {
                handler?.Invoke(sender, args);
            }
        }

        public void Update(GameTime gameTime)
        {
            // update and fire timed events
            _elapsedTime += gameTime.ElapsedGameTime;

            // fire next frame events
            var queuesize = _nextFrameEvents.Count;
            for(int i = 0; i < queuesize; i++)
            {
                var evt = _nextFrameEvents.Dequeue();
                FireEvent(evt.Sender, evt.Id, evt.Args);
            }


            var seconds = _elapsedTime.TotalSeconds;
            while (_timedEvents.Count > 0 && _timedEvents.First.Time <= _elapsedTime)
            {
                var evt = _timedEvents.Dequeue();
                FireEvent(evt.Sender, evt.Id, evt.Args);
            }
        }

        public void FireEventNextFrame(object sender, int id, EventArgs args)
        {
            _nextFrameEvents.Enqueue(new QueuedEvent(sender, id, args));
        }

        public void RemoveEventListener(int id, EventHandler handler)
        {
            _handlers[id] -= handler;
        }

        public void FireEventAfter(object sender, int id, EventArgs args, TimeSpan time)
        {
            var newTime = _elapsedTime + time;
            _timedEvents.Enqueue(new TimedQueuedEvent(sender, id, args, newTime), newTime.TotalSeconds);
        }

        public void CancelAllTimedEvents()
        {
            _timedEvents.Clear();
        }

        public void CancelAllNextFrameEvents()
        {
            _nextFrameEvents.Clear();
        }
    }
}
