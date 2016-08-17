using Artemis.Interface;

using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using FellSky.Framework;
using Artemis;
using FellSky.Components;
using FellSky.Systems.SceneGraphRenderers;
using FellSky.Services;
using FellSky.Systems;

namespace FellSky.Game.Ships.Parts
{
    public enum HullColorType
    {
        Hull, Base, Trim
    }

    /// <summary>
    /// The physical parts of a ship. Determines the graphics and collision data of the ship.
    /// </summary>
    public sealed class Hull: ShipPart, IComponent
    {
        public Hull() { }

        public Hull(string id, Vector2 position, float rotation, Vector2 scale, Vector2 origin, Color color)
        {
            SpriteId = id;
            Transform.Position = position;
            Transform.Scale = scale;
            Transform.Rotation = rotation;
            Transform.Origin = origin;
            Color = color;
        }

        public HullColorType ColorType { get; set; } = HullColorType.Hull;
        public string ShapeId { get; set; }
        //public SpriteEffects SpriteEffect { get; set; }
        public float Health { get; set; } = 100;

        public override Entity CreateEntity(EntityWorld world, Entity ship, Entity parent, int? index=null)
        {
            var entity = world.CreateEntity();
            parent.AddChild(entity, index);
            var hull = new HullComponent(this, ship);
            entity.AddComponent<IShipPartComponent>(hull);
            entity.AddComponent(hull);
            entity.AddComponent(new HealthComponent(hull.Part.Health));
            entity.AddSceneGraphRendererComponent<StandardShipModelRenderer>();
            entity.AddComponent(Transform.Clone());
            var spriteManager = ServiceLocator.Instance.GetService<ISpriteManagerService>();
            var spriteComponent = spriteManager.CreateSpriteComponent(SpriteId);
            entity.AddComponent(spriteComponent);
            entity.AddComponent(new BoundingBoxComponent(new FloatRect(0, 0, spriteComponent.TextureRect.Width, spriteComponent.TextureRect.Height)));

            if (parent.HasComponent<RigidBodyComponent>() && !Flags.Contains("NoPhysics"))
            {
                var physics = world.SystemManager.GetSystem<PhysicsSystem>();
                RigidBodyComponent body;
                body = ship?.GetComponent<RigidBodyComponent>();
                var sprite = spriteManager.Sprites[SpriteId];
                var origin = new Vector2(sprite.OriginX ?? sprite.W / 2, sprite.OriginY ?? sprite.H / 2);
                var factor = new Vector2(1f / Constants.PhysicsUnitScale);
                var matrix = //Matrix.CreateTranslation(new Vector3(-0.5f,-0.5f,0)) *
                             Matrix.CreateScale(new Vector3(factor, 1)) *
                             Matrix.CreateTranslation(new Vector3(-Transform.Origin, 0)) *
                             Matrix.CreateScale(new Vector3(Transform.Scale, 1)) *
                             Matrix.CreateRotationZ(Transform.Rotation) *
                             Matrix.CreateTranslation(new Vector3(Transform.Position, 0)) *
                             Matrix.CreateScale(Constants.PhysicsUnitScale);

                var fixtureComponent = physics.CreateAndAttachFixture(ship.GetComponent<RigidBodyComponent>(), ShapeId ?? SpriteId, matrix);
                foreach (var fixture in fixtureComponent.Fixtures)
                {
                    fixture.UserData = entity;
                }
                entity.AddComponent(fixtureComponent);

            }
            entity.Refresh();
            return entity;
        }
    }


    
}
