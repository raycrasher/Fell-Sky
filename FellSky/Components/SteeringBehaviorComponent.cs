using Artemis.Interface;
using FellSky.Framework.SteeringBehaviors;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FellSky.Components
{
    public class SteeringBehaviorComponent: IComponent
    {
        public List<SteeringBehavior> Behaviors = new List<SteeringBehavior>();
        public float[] DangerMap;
        public float[] InterestMap;

        // output
        public Vector2 DesiredMovementVector;
        public float DesiredFacing;
    }
}
