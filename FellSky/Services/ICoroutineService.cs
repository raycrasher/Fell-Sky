using System;
using System.Collections;

namespace FellSky.Services
{
    public interface ICoroutineService
    {
        void RunCoroutines(TimeSpan timestep);
        Coroutine StartCoroutine(IEnumerable routine);
    }
}