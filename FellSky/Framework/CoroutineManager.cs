using System;
using System.Collections;
using System.Collections.Generic;

namespace FellSky.Framework
{
    public class CoroutineManager
    {
        private LinkedList<Coroutine> Coroutines = new LinkedList<Coroutine>();

        public Coroutine StartCoroutine(IEnumerable routine)
        {
            var r = new Coroutine(routine);
            Coroutines.AddLast(r);
            r.Resume();
            return r;
        }

        public void RunCoroutines(TimeSpan timestep)
        {
            LinkedListNode<Coroutine> node = Coroutines.First;
            while (node != null)
            {
                var thisNode = node;
                node = node.Next;

                if (thisNode.Value.Status == CoroutineStatus.Stopped)
                {
                    Coroutines.Remove(thisNode);
                    continue;
                }

                if (!thisNode.Value.Run(timestep))
                {
                    Coroutines.Remove(thisNode);
                    if (thisNode.Value.OnDone != null)
                        thisNode.Value.OnDone();
                }
            }
        }
    }

}
