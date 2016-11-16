using Artemis;
using Artemis.Interface;
using FellSky.Game.Ships.Parts;
using FellSky.Game.Ships;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using FellSky.Services;

namespace FellSky.Components
{
    public class ShipComponent: IComponent
    {
        

        public Ship Ship { get; set; }
        public ShipModel ShipModel { get; set; }

        public List<Entity> Thrusters { get; set; } = new List<Entity>();
        public List<Entity> Weapons { get; set; } = new List<Entity>();
        public List<Entity> Hardpoints { get; set; } = new List<Entity>();

        public float AngularTorque { get; set; }
        public Vector2 LinearThrustVector { get; set; }
        public bool AttemptBoost { get; set; }
        public Vector2 CenterOfMass { get; set; }

        public ShipVariant Variant { get; set; }
        
        // calculated values
        public float MaxForwardSpeed { get; internal set; }
        public float MaxManeuverSpeed { get; internal set; }
        public FloatRect BoundingBox { get; internal set; }
        public float Radius { get; internal set; }
        public float Mass { get; internal set; }

        // functions

        public ShipComponent(Ship ship)
        {
            Ship = ship;
        }

        public void RecalculateValues(Entity entity)
        {
            var body = entity.GetComponent<RigidBodyComponent>();

            Mass = (body?.Body?.Mass ?? 1f) / Constants.PhysicsUnitScale;

            BoundingBox = ShipModel.CalculateBoundingBox(0);
            Radius = Math.Max(BoundingBox.Width, BoundingBox.Height) / 2;
            MaxForwardSpeed = UtilityExtensions.GetMaxLinearVelocity(Mass, 10, Ship.Handling.LinearDamping, Ship.Handling.ForwardForce / Constants.PhysicsUnitScale);
            MaxManeuverSpeed = UtilityExtensions.GetMaxLinearVelocity(Mass, 10, Ship.Handling.LinearDamping, Ship.Handling.ManeuverForce / Constants.PhysicsUnitScale);
        }
    }
}
