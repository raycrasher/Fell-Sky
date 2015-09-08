using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Framework
{
    public class StateManager<TStateBase, TEventBase>
    {
        bool _isStarted = false;

        public TStateBase CurrentState { get; private set; }

        List<Tuple<Type, Type, Func<TStateBase, TStateBase, bool>>> Transitions = new List<Tuple<Type, Type, Func<TStateBase, TStateBase, bool>>>();
        List<Tuple<Type, Type, Action<TStateBase, TEventBase>>> Events = new List<Tuple<Type, Type, Action<TStateBase, TEventBase>>>();
        Stack<TStateBase> _stateStack = new Stack<TStateBase>();

        public void Start<T>(T state)
            where T : TStateBase
        {
            if (_isStarted) throw new InvalidOperationException("Scene Manager already started.");
            CurrentState = state;
        }

        public bool PushState<T>(T next)
            where T : TStateBase
        {            
            var result = Transitions.Find(t => t.Item1.GetType() == typeof(T) && t.Item2 == CurrentState.GetType())?.Item3?.Invoke(CurrentState,next);
            if(result == true)
            {
                _stateStack.Push(CurrentState);
                CurrentState = next;
            }
            return result == true;
        }

        public bool PopState()
        {
            var state = _stateStack.Peek();
            var result = Transitions.Find(t => t.Item1.GetType() == CurrentState.GetType() && t.Item2 == state.GetType())?.Item3?.Invoke(CurrentState, state);
            if(result == true)
            {
                CurrentState = _stateStack.Pop();
            }
            return result == true;
        }

        public StateManager<TStateBase, TEventBase> AddTransition<TCurrent, TNext>(Func<TStateBase, TStateBase, bool> func)
            where TCurrent : TStateBase
            where TNext : TStateBase
        {
            Transitions.Add(Tuple.Create(typeof(TCurrent), typeof(TNext), func));
            return this;
        }

        public StateManager<TStateBase, TEventBase> AddEventHandler<TState,TEvent>(Action<TStateBase, TEventBase> func)
            where TState : TStateBase
            where TEvent : TEventBase
        {
            Events.Add(Tuple.Create(typeof(TState), typeof(TEvent), func));
            return this;
        }
    }
}
