using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Framework
{
    public enum CoroutineStatus { Stopped, Playing, Paused }

    public class Coroutine
    {
        public Action OnDone { get; set; }

        private IEnumerator Routine;
        public CoroutineStatus Status { get; private set; }

        ICoroutineYieldValue _yieldValue;

        public Coroutine(IEnumerable routine)
        {
            // TODO: Complete member initialization
            Status = CoroutineStatus.Paused;
            this.Routine = routine.GetEnumerator();
        }

        /// <summary>
        /// Stop the coroutine
        /// </summary>
        public void Stop()
        {
            if (Status != CoroutineStatus.Stopped)
            {
                Status = CoroutineStatus.Stopped;
            }
        }

        /// <summary>
        /// Resume the coroutine
        /// </summary>
        public void Resume()
        {
            if (Status == CoroutineStatus.Paused)
            {
                Status = CoroutineStatus.Playing;
            }
        }

        /// <summary>
        /// Pause the coroutine
        /// </summary>
        public void Paused()
        {
            if (Status == CoroutineStatus.Playing)
            {
                Status = CoroutineStatus.Paused;
            }
        }

        protected internal virtual bool Run(TimeSpan timestep)
        {
            _yieldValue = Routine.Current as ICoroutineYieldValue;

            Status = CoroutineStatus.Playing;
            if (_yieldValue != null)
            {
                if (!_yieldValue.Step(this, timestep))
                {
                    return true;
                }
            }
            return Routine.MoveNext();
        }



        public static ICoroutineYieldValue WaitFor(TimeSpan ts)
        {
            return new YieldWaitFor { Delay = ts };
        }

        public static ICoroutineYieldValue WaitUntil(Func<bool> condition)
        {
            return new YieldWaitUntil { Condition = condition };
        }

        public interface ICoroutineYieldValue
        {
            bool Step(Coroutine routine, TimeSpan time);
        }

        class YieldWaitUntil : ICoroutineYieldValue
        {
            public Func<bool> Condition;
            public bool Step(Coroutine routine, TimeSpan time)
            {
                return Condition();
            }
        }

        class YieldWaitFor : ICoroutineYieldValue
        {
            public TimeSpan Delay;

            public bool Step(Coroutine routine, TimeSpan time)
            {
                if (Delay <= TimeSpan.Zero) return true;
                Delay -= time;
                return false;
            }
        }
    }

}
