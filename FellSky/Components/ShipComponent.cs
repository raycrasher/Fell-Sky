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
        /// <summary>
        /// The Ship data model.
        /// </summary>
        public Ship Ship { get; set; }

        public List<PartEntityPair> PartEntities { get; set; } = new List<PartEntityPair>();
        public IReadOnlyList<PartEntityPair> Thrusters { get; set; } = new List<PartEntityPair>();

        /// <summary>
        /// The angular thrust vector. Negative for CCW, positive for CW. Zero for no torque. 
        /// </summary>
        public float AngularTorque { get; set; }

        /// <summary>
        /// The linear thrust vector, in world space. Thrust percentage is given by length (-1 ~ 0 ~ 1)
        /// </summary>
        public Vector2 LinearThrustVector { get; set; }

        /// <summary>
        /// Attempt to provide boost during maneuver. Has no effect if boost capability is missing.
        /// </summary>
        public bool AttemptBoost { get; set; }
        
        /// <summary>
        /// Center of mass. Usually is equal to the COM of the main rigid body.
        /// </summary>
        public Vector2 CenterOfMass { get; set; }

        public List<Entity> AdditionalRigidBodyEntities { get; set; } = new List<Entity>();

        // functions

        public ShipComponent(Ship ship)
        {
            Ship = ship;
        }

        
    }
}
