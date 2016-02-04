using Artemis.Interface;
using FellSky.Framework.SteeringBehaviors;

namespace FellSky.Components
{
    public class SteeringBehaviorComponent<T>: IComponent
        where T : ISteeringBehavior
    {
        public T Behavior { get; set; }
    }
}
