using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using FellSky.Components;
using Microsoft.Xna.Framework;
using static FellSky.Utilities;

namespace FellSky.Systems
{
    public class ShipUpdateSystem: Artemis.System.ParallelEntityProcessingSystem
    {
        public ShipUpdateSystem()
            : base (Aspect.All(typeof(ShipComponent)))
        { }


        /// <summary>
        /// Event handler. Also removes all child entities of the ship.
        /// </summary>
        /// <param name="entity"></param>
        public override void OnRemoved(Entity entity)
        {
            var shipComponent = entity.GetComponent<ShipComponent>();
            if (shipComponent != null)
            {
                foreach (var child in shipComponent.PartEntities)
                {
                    child.Entity.Delete();
                }
            }

            base.OnRemoved(entity);
        }

        public override void Process(Entity entity)
        {
            var shipComponent = entity.GetComponent<ShipComponent>();
            if (entity.HasComponent<RigidBodyComponent>())
            {
                var rigidBody = entity.GetComponent<RigidBodyComponent>();
                rigidBody.Body.ApplyForce(rigidBody.Body.GetWorldVector(shipComponent.LinearThrustVector), rigidBody.Body.WorldCenter);
                rigidBody.Body.ApplyTorque(shipComponent.AngularTorque);
            }
        }

        private void UpdateThrusters(Entity ship, IList<Entity> thrusterEntities)
        {
            var shipComponent = ship.GetComponent<ShipComponent>();
            var thrusters = thrusterEntities.Select(t => t.GetComponent<ThrusterComponent>());
            var xform = ship.GetComponent<Transform>();

            bool isShipTurning = Math.Abs(shipComponent.AngularTorque) <= float.Epsilon;


            foreach(var thruster in thrusters)
            {
                bool isThrusting = false;

                var offset = Math.Abs(MathHelper.WrapAngle(shipComponent.LinearThrustVector.ToAngleRadians() - xform.Rotation - thruster.Part.Transform.Rotation) / Math.PI);
                if (offset < thruster.AngleCutoff) isThrusting = true;

                if( thruster.Part.ThrusterType == Game.Ships.Parts.ThrusterType.Maneuver)
                {

                }

                thruster.IsThrusting = isThrusting;
            }
        }
    }
}
