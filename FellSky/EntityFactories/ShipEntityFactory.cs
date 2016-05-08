using Artemis;
using FellSky.Game.Ships.Parts;
using FellSky.Components;
using FellSky.Services;
using FellSky.Game.Ships;
using Microsoft.Xna.Framework;
using FellSky.Systems;
using System;
using Artemis.Interface;
using System.Linq;
using System.Collections.Generic;

namespace FellSky.EntityFactories
{
    public class ShipEntityFactory
    {
        private ISpriteManagerService _spriteManager;

        public EntityWorld World { get; set; }

        public ShipEntityFactory(EntityWorld world, ISpriteManagerService spriteManager)
        {
            _spriteManager = spriteManager;
            World = world;
        }
        /*
        public Entity CreateShipEntity(string name, Vector2 position, float rotation = 0, bool addPhysics = true)
        {

        }
        */

        public void UpdateShipComponentPartList(Entity ship, bool addPhysics=false)
        {
            var component = ship.GetComponent<ShipComponent>();
            var entityLookup = component.PartEntities.ToDictionary(k => k.Part);
            component.PartEntities.Clear();

            foreach (var part in component.Ship.Parts)
            {
                PartEntityPair pe;
                if (entityLookup.TryGetValue(part, out pe))
                    if(pe.Entity.IsActive && pe.Entity.IsEnabled)
                        component.PartEntities.Add(pe);
                else
                    CreatePartEntity(ship, part, addPhysics);
            }
        }

        public Entity CreatePartEntity(Entity ship, ShipPart part, bool addPhysics = false, int index = -1)
        {
            Entity e;
            if (part is Hull) e = CreateHullEntity(ship, (Hull)part, addPhysics, index);
            else if (part is Thruster) e = CreatePartEntity(ship, new ThrusterComponent((Thruster)part, ship), index);
            else throw new NotImplementedException();
            var shipComponent = ship.GetComponent<ShipComponent>();
            if (index < 0)
                shipComponent.Ship.Parts.Add(part);
            else
                shipComponent.Ship.Parts.Insert(index, part);
            return e;
        }
        
        public Entity CreateShipEntity(Ship ship, Vector2 position, float rotation=0, bool addPhysics=false)
        {
            var shipEntity = World.CreateEntity();
            shipEntity.AddComponent(new ShipComponent(ship));
            shipEntity.AddComponent(new Transform());

            if (addPhysics)
            {
                var physics = World.SystemManager.GetSystem<PhysicsSystem>();
                shipEntity.AddComponent(physics.CreateRigidBody(position, rotation));
            }

            foreach(var part in ship.Parts)
                CreatePartEntity(shipEntity, part, addPhysics);            

            return shipEntity;
        }

        private Entity CreatePartEntity<TComponent>(Entity ship, TComponent component, int index = -1)
            where TComponent: IShipPartComponent, IComponent
        {
            var part = component.Part;
            var entity = World.CreateEntity();
            entity.AddComponent(component);
            entity.AddComponent(new LocalTransformComponent(ship));
            entity.AddComponent(part.Transform);
            var shipComponent = ship.GetComponent<ShipComponent>();
            if (index < 0)
                shipComponent.PartEntities.Add(new PartEntityPair(component.Part, entity));
            else
                shipComponent.PartEntities.Insert(index, new PartEntityPair(component.Part, entity));

            if (!string.IsNullOrEmpty(part.SpriteId))
            {
                var spriteComponent = _spriteManager.CreateSpriteComponent(part.SpriteId);
                entity.AddComponent(spriteComponent);
                entity.AddComponent(new BoundingBoxComponent(new FloatRect(0, 0, spriteComponent.TextureRect.Width, spriteComponent.TextureRect.Height)));
            }
            return entity;
        }

        private Entity CreateHullEntity(Entity ship, Hull hull, bool addPhysics, int index)
        {
            var entity = CreatePartEntity(ship, new HullComponent(hull, ship), index);
            if (addPhysics)
            {
                var physics = World.SystemManager.GetSystem<PhysicsSystem>();
                RigidBodyComponent body;
                if (hull.PhysicsBodyIndex == 0)
                    body = ship?.GetComponent<RigidBodyComponent>();
                else
                    body = ship?.GetComponent<ShipComponent>()?.AdditionalRigidBodyEntities[hull.PhysicsBodyIndex].GetComponent<RigidBodyComponent>();
                entity.AddComponent(physics.CreateAndAttachFixture(ship.GetComponent<RigidBodyComponent>(), hull.ShapeId, hull.Transform));
            }
            return entity;
        }
    }
}
