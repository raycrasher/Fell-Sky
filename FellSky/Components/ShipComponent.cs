using Artemis;
using Artemis.Interface;
using FellSky.Models.Ships.Parts;
using FellSky.Ships;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace FellSky.Components
{
    public class ShipComponent: IComponent
    {
        public Ship Ship { get; set; }
        public List<Entity> HullEntities { get; set; } = new List<Entity>();
        public List<Entity> ThrusterEntities { get; set; } = new List<Entity>();

        public IEnumerable<Entity> ChildEntities => HullEntities.Concat(ThrusterEntities);

        // control
        public float DesiredTorque { get; set; }
        public Vector2 LinearThrustVector { get; set; }
        public bool AttemptBoost { get; set; }
        public IEnumerable<Entity> PartEntities => HullEntities.Concat(ThrusterEntities);
        public Vector2 CenterOfMass { get; set; }

        // functions

        public ShipComponent(Ship ship)
        {
            Ship = ship;
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

        public Entity[] ThrusterAssignmentsCCW { get; set; }
        public Entity[] ThrusterAssignmentsCW { get; set; }
        public Entity[] ThrusterAssignmentsMain { get; set; }

        public void UpdateThrusterAssignments()
        {
            
        }
    }
}
