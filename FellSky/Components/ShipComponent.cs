using Artemis;
using Artemis.Interface;
using FellSky.Game.Ships.Parts;
using FellSky.Game.Ships;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace FellSky.Components
{
    public class ShipComponent: IComponent
    {

        /// <summary>
        /// The Ship data model.
        /// </summary>
        public Ship Ship { get; set; }

        /// <summary>
        /// Hull entities.
        /// </summary>
        public List<Entity> HullEntities { get; set; } = new List<Entity>();

        /// <summary>
        /// Thruster entities.
        /// </summary>
        public List<Entity> ThrusterEntities { get; set; } = new List<Entity>();

        /// <summary>
        /// Child entities (parts, thrusters, etc.) This is a readonly IEnumerable.
        /// </summary>
        public IEnumerable<Entity> ChildEntities => HullEntities.Concat(ThrusterEntities);

        /// <summary>
        /// The angular thrust vector. Negative for CCW, positive for CW. Zero for no torque. 
        /// </summary>
        public float AngularThrustVector { get; set; }

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

        public void RemovePart(Entity e)
        {
            var part = e.Components.OfType<IShipPartComponent>().First().Part;
            IList<Entity> list =
                part is Hull ? HullEntities :
                part is Thruster ? ThrusterEntities :
                null;
            Ship.RemovePart(part);
            list.Remove(e);
            e.Delete();
        }

        public void RemovePart<T>(T part)
            where T: ShipPart
        {
            IList<Entity> list =
                part is Hull ? HullEntities :
                part is Thruster ? ThrusterEntities :
                null;

            if (list == null) throw new InvalidOperationException("Part type is invalid");

            var entity = list.First(s => s.Components.OfType<ShipPartComponent<T>>().First().Part == part);
            Ship.RemovePart(part);
            list.Remove(entity);
            entity.Delete();
        }
    }
}
