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
    public static class ShipEntityFactory
    {
        private static ISpriteManagerService SpriteManager => ServiceLocator.Instance.GetService<ISpriteManagerService>();


        public static void UpdateShipComponentPartList(EntityWorld world, Entity ship, bool addPhysics=false)
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
                    CreatePartEntity(world, ship, part, addPhysics);
            }
        }

        public static Entity AddAndCreatePartEntity(EntityWorld world, Entity ship, ShipPart part, bool addPhysics, int index = -1)
        {
            var e = CreatePartEntity(world, ship, part, addPhysics, index);
            var shipComponent = ship.GetComponent<ShipComponent>();
            if (index < 0)
                shipComponent.Ship.Parts.Add(part);
            else
                shipComponent.Ship.Parts.Insert(index, part);
            return e;
        }

        public static Entity CreatePartEntity(EntityWorld world, Entity ship, ShipPart part, bool addPhysics = false, int index = -1)
        {
            Entity e;
            if (part is Hull) e = CreateHullEntity(world, ship, (Hull)part, addPhysics, index);
            else if (part is Thruster) e = CreatePartEntity(world, ship, new ThrusterComponent((Thruster)part, ship), index);
            else throw new NotImplementedException();
            return e;
        }
        
        public static Entity CreateShipEntity(this EntityWorld world, Ship ship, Vector2 position, float rotation=0, bool addPhysics=false)
        {
            var shipEntity = world.CreateEntity();
            shipEntity.AddComponent(new ShipComponent(ship));
            shipEntity.AddComponent(new Transform());

            if (addPhysics)
            {
                var physics = world.SystemManager.GetSystem<PhysicsSystem>();
                shipEntity.AddComponent(physics.CreateRigidBody(position, rotation));
                var rigidBody = shipEntity.GetComponent<RigidBodyComponent>();
                rigidBody.Body.IsStatic = false;
                rigidBody.Body.LinearDamping = ship.Handling.LinearDamping;
                rigidBody.Body.AngularDamping = ship.Handling.AngularDamping;
            }

            foreach(var part in ship.Parts)
                CreatePartEntity(world, shipEntity, part, addPhysics);

            if (addPhysics)
            {
                var rigidBody = shipEntity.GetComponent<RigidBodyComponent>();
                rigidBody.Body.ResetMassData();
            }

            UpdateShipComponentPartTypeLists(shipEntity);

            return shipEntity;
        }

        public static void UpdateShipComponentPartTypeLists(Entity shipEntity)
        {
            var component = shipEntity.GetComponent<ShipComponent>();
            component.Thrusters = component.PartEntities.Where(p => p.Entity.HasComponent<ThrusterComponent>()).ToList();
        }

        private static Entity CreatePartEntity<TComponent>(EntityWorld world, Entity ship, TComponent component, int index = -1)
            where TComponent: IShipPartComponent, IComponent
        {
            var part = component.Part;
            var entity = world.CreateEntity();
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
                var spriteComponent = SpriteManager.CreateSpriteComponent(part.SpriteId);
                entity.AddComponent(spriteComponent);
                entity.AddComponent(new BoundingBoxComponent(new FloatRect(0, 0, spriteComponent.TextureRect.Width, spriteComponent.TextureRect.Height)));
            }
            return entity;
        }

        private static Entity CreateHullEntity(EntityWorld world, Entity ship, Hull hull, bool addPhysics, int index)
        {
            var entity = CreatePartEntity(world, ship, new HullComponent(hull, ship), index);
            if (addPhysics)
            {
                var physics = world.SystemManager.GetSystem<PhysicsSystem>();
                RigidBodyComponent body;
                if (hull.PhysicsBodyIndex == 0)
                    body = ship?.GetComponent<RigidBodyComponent>();
                else
                    body = ship?.GetComponent<ShipComponent>()?.AdditionalRigidBodyEntities[hull.PhysicsBodyIndex].GetComponent<RigidBodyComponent>();

                var sprite = SpriteManager.Sprites[hull.SpriteId];
                var origin = new Vector2(sprite.OriginX ?? sprite.W/2, sprite.OriginY ?? sprite.H/2);
                var factor = new Vector2(1f/Constants.PhysicsUnitScale) + origin;
                var matrix = //Matrix.CreateTranslation(new Vector3(-0.5f,-0.5f,0)) *
                             Matrix.CreateScale(new Vector3(factor,1)) * 
                             Matrix.CreateTranslation(new Vector3(-hull.Transform.Origin, 0)) *
                             Matrix.CreateScale(new Vector3(hull.Transform.Scale, 1)) *
                             Matrix.CreateRotationZ(hull.Transform.Rotation) *
                             Matrix.CreateTranslation(new Vector3(hull.Transform.Position,0)) *
                             Matrix.CreateScale(Constants.PhysicsUnitScale);
                             

                entity.AddComponent(physics.CreateAndAttachFixture(ship.GetComponent<RigidBodyComponent>(), hull.ShapeId ?? hull.SpriteId, matrix));
            }
            return entity;
        }
    }
}
