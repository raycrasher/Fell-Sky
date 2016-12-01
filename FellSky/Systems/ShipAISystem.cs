using Artemis;
using FellSky.Components;
using Microsoft.Xna.Framework;
using SharpSteer2;
using SharpSteer2.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis.Interface;
using SharpSteer2.Database;

namespace FellSky.Systems
{
    public enum ShipAIType
    {
        Assault,    
        // tries to bring the most guns to bear
        // will flank if able

        Guardian,
        // will go in front of vulnerable ships and absorb fire

        Flanker,

    }

    public class ShipAISystem: Artemis.System.EntitySystem
    {
        public ShipAISystem() :
            base(Aspect.All(typeof(ShipComponent), typeof(ShipAIComponent), typeof(SpatialTokenComponent)))
        { }


        public IProximityDatabase<IVehicle> ProximityDatabase { get; private set; }

        public HashSet<Entity> Ships { get; } = new HashSet<Entity>();

        public List<Entity> _neighborSearchResults = new List<Entity>();

        public override void LoadContent()
        {
            EntityWorld.EntityManager.AddedEntityEvent += OnAddEntity;
            EntityWorld.EntityManager.RemovedEntityEvent += OnRemoveEntity;
        }

        private void OnRemoveEntity(Entity entity)
        {
            var vehicle = entity.GetComponent<VehicleComponent>();
            if(vehicle!=null && entity.HasComponent<ShipComponent>())
            {
                Ships.Remove(entity);
            }
        }

        private void OnAddEntity(Entity entity)
        {
            var vehicle = entity.GetComponent<VehicleComponent>();
            if (vehicle != null && entity.HasComponent<ShipComponent>())
            {
                var ship = entity.GetComponent<ShipComponent>();
                ship.RecalculateValues(entity);
                Ships.Add(entity);
            }
        }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            foreach(var entity in entities.Values)
            {
                var ai = entity.GetComponent<ShipAIComponent>();
                var vehicle = entity.GetComponent<VehicleComponent>();
                UpdateVehicle(entity);

                switch (ai.Mode)
                {
                    case ShipAIMode.Travel:
                        DoTravel(entity, ai.MainTarget.GetComponent<Transform>().Position);
                        break;
                }
                
            }
        }

        private void UpdateVehicle(Entity entity)
        {
            var vehicle = entity.GetComponent<VehicleComponent>();
            var body = entity.GetComponent<RigidBodyComponent>();
            vehicle.Position = body.WorldPosition.ToNumericsVector3();
            var velocity = body.Body.LinearVelocity / Constants.PhysicsUnitScale;
            vehicle.Speed = velocity.Length();
            vehicle.Forward = velocity.ToNumericsVector3();
        }

        private void DoTravel(Entity entity, Vector2 position)
        {
            var vehicle = entity.GetComponent<VehicleComponent>();
            var ship = entity.GetComponent<ShipComponent>();
            var mass = ship.Mass;
            
            vehicle.SteerForArrival(position.ToNumericsVector3(), ship.MaxForwardSpeed, GetDistanceToChangeSpeed(mass, -vehicle.Speed, ship.Ship.Handling.ManeuverForce / Constants.PhysicsUnitScale) + ship.Radius);
            ship.LinearThrustVector = vehicle.Velocity.ToXnaVector2();
            var spatialToken = entity.GetComponent<SpatialTokenComponent>();
            var xform = entity.GetComponent<Transform>();
            spatialToken.Token.FindNeighbors(xform.Position.ToNumericsVector3(), vehicle.Radius * 3, _neighborSearchResults);
            vehicle.SteerToAvoidCloseNeighbors(ship.Radius, _neighborSearchResults.Select(e=>e.GetComponent<VehicleComponent>()));
        }

        private float GetDistanceToChangeSpeed(float mass, float deltaV, float force)
        {
            var accel = force / mass;
            var time = deltaV / accel;
            return 0.5f * accel * time * time;
        }

        protected void Harass(Entity ship)
        {
            var ai = ship.GetComponent<ShipAIComponent>();
            var target = ai.MainTarget;           
        }
    }
}
