using Artemis.Interface;
using FellSky.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public class CoroutineComponent: IComponent
    {

        public CoroutineComponent(IEnumerable coroutine, Action onDone=null)
        {
            CoroutineFunction = coroutine;
        }

        public Coroutine Coroutine;
        public readonly IEnumerable CoroutineFunction;
        public readonly Action OnDone;
    }
}
