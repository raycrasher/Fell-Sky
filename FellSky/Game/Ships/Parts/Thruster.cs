
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Artemis;
using FellSky.Components;
using FellSky.Services;
using FellSky.Systems.SceneGraphRenderers;

namespace FellSky.Game.Ships.Parts
{
    public enum ThrusterType
    {
        Main, Maneuver, Boost
    }

    public class Thruster: ShipPart
    {
        public float Length { get; set; }
        public float Width { get; set; }
        public string ParticleTrailId { get; set; } // not required, can be null for no particle trail.
        public ThrusterType ThrusterType { get; set; }

        public float MaxThrust { get; set; }
        public bool IsIdleModeOnZeroThrust { get; set; }
        public TimeSpan RampUpTime { get; set; }
        public TimeSpan RampDownTime { get; set; }

        public Thruster()
        {
            Color = Color.White;
        }

        public Thruster(string id, Vector2 position, float rotation, Vector2 scale, Vector2 origin, Color color)
        {
            SpriteId = id;
            Transform.Position = position;
            Transform.Scale = scale;
            Transform.Rotation = rotation;
            Transform.Origin = origin;
            Color = color;
        }

        public override Entity CreateEntity(EntityWorld world, Entity ship, Entity parent, int? index=null)
        {
            var entity = world.CreateEntity();
            parent.AddChild(entity, index);
            var thruster = new ThrusterComponent(this, ship);
            entity.AddComponent<IShipPartComponent>(thruster);
            entity.AddComponent(thruster);
            entity.AddComponent<ISceneGraphRenderableComponent<StandardShipRenderer>>(new ThrusterRendererComponent());
            entity.AddComponent(Transform);
            var spriteManager = ServiceLocator.Instance.GetService<ISpriteManagerService>();
            var spriteComponent = spriteManager.CreateSpriteComponent(SpriteId);
            entity.AddComponent(spriteComponent);
            entity.AddComponent(new BoundingBoxComponent(new FloatRect(0, 0, spriteComponent.TextureRect.Width, spriteComponent.TextureRect.Height)));
            ship.GetComponent<ShipComponent>().Thrusters.Add(entity);
            return entity;
        }
    }
}
