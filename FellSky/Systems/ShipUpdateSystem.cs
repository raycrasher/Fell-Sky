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

        public override void Process(Entity ship)
        {
            var shipComponent = ship.GetComponent<ShipComponent>();
            if (ship.HasComponent<RigidBodyComponent>())
            {
                var rigidBody = ship.GetComponent<RigidBodyComponent>();
                rigidBody.Body.ApplyForce(rigidBody.Body.GetWorldVector(shipComponent.LinearThrustVector), rigidBody.Body.WorldCenter);
                rigidBody.Body.ApplyTorque(shipComponent.AngularTorque);
            }          
            UpdateThrusters(ship);

            shipComponent.AngularTorque = 0;
            shipComponent.LinearThrustVector = Vector2.Zero;
        }

        private void UpdateThrusters(Entity ship)
        {
            var shipComponent = ship.GetComponent<ShipComponent>();
            var thrusters = shipComponent.Thrusters.Select(pe => pe.Entity.GetComponent<ThrusterComponent>());
            var xform = ship.GetComponent<Transform>();

            bool isShipTurning = Math.Abs(shipComponent.AngularTorque) > 0;
            var isShipThrusting = shipComponent.LinearThrustVector.LengthSquared() > 0;

            foreach (var thruster in thrusters)
            {
                bool isThrusting = false;
                if (isShipThrusting)
                {
                    var offset = Math.Abs(GetLesserAngleDifference(shipComponent.LinearThrustVector.ToAngleRadians() - MathHelper.Pi / 2f, thruster.Part.Transform.Rotation));
                    isThrusting = offset < 1;
                }
                if (isShipTurning)
                {
                    var rotateDir = (AngularDirection)Math.Sign(shipComponent.AngularTorque);
                    //isThrusting = rotateDir == thruster.RotateDir;
                    isThrusting |= thruster.GetAngularThrustMult(rotateDir, Vector2.Zero) > 0;
                }

                thruster.IsThrusting = isThrusting;
                if (isThrusting)
                    thruster.ThrustPercentage = MathHelper.Clamp(thruster.ThrustPercentage + 0.05f, 0, 1);
                else
                    thruster.ThrustPercentage = MathHelper.Clamp(thruster.ThrustPercentage - 0.05f, 0, 1);
            }
        }
    }
}
