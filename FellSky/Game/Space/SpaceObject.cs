using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Artemis;
using System;
using FellSky.Components;
using FellSky.Services;
using Microsoft.Xna.Framework.Graphics;

namespace FellSky.Game.Space
{
    public class SpaceObject
    {
        public SpaceObject() { }
        public SpaceObject(params SpaceObject[] children)
        {
            Children.AddRange(children);
        }

        public Vector2 Position { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string TextureId { get; set; }
        public string IconSpriteId { get; set; }

        public float OrbitEpoch { get; set; }
        public float OrbitRadius { get; set; } = 1;   // astronomical units
        public float OrbitPeriod { get; set; } = 365; // earth days

        public Vector2 Color { get; set; }

        public float Mass { get; set; }

        public List<SpaceObject> Children { get; set; } = new List<SpaceObject>();

        public virtual Entity CreateEntity(EntityWorld world)
        {
            var entity = world.CreateEntity();
            entity.AddComponent(new SpaceObjectComponent(this));
            var xform = entity.AddComponentFromPool<Transform>();
            var sprite = ServiceLocator.Instance.GetService<ISpriteManagerService>().CreateSpriteComponent(TextureId);
            xform.Origin = sprite.Origin;
            
            entity.AddComponent(sprite);

            foreach (var child in Children)
            {
                var childEntity = child.CreateEntity(world);
                entity.AddChild(childEntity);
            }

            return entity;
        }

        public Vector2 GetPositionAtTime(DateTime time)
        {
            var numDays = time - Constants.EpochJ2000;
            var angle = numDays.TotalDays / OrbitPeriod;
            return Utilities.CreateVector2FromAngle((float)angle) * OrbitRadius; 
        }
    }
}
