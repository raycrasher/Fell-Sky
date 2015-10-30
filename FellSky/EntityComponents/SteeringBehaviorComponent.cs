using Artemis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FellSky.Framework.SteeringBehaviors;
using Microsoft.Xna.Framework;
using FellSky.Systems;

namespace FellSky.EntityComponents
{
    public class SteeringBehaviorComponent: IComponent
    {
        public ITransform Transform { get; set; }
        public Vector2 Velocity { get; set; }

        public float MaxSpeed { get; }
        public float MaxLinearForce { get; }

        public Vector2 DesiredMovementVector { get; set; }

        public List<ISteeringBehavior> Behaviors { get; } = new List<ISteeringBehavior>();

        public float[] DangerMap { get; } = new float[SteeringBehaviorSystem.SteeringMapSize];
        public float[] InterestMap { get; } = new float[SteeringBehaviorSystem.SteeringMapSize];
    }
}
