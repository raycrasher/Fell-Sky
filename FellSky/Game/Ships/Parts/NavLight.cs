using Artemis.Interface;
using Microsoft.Xna.Framework;
using System;
using Artemis;
using FellSky.Components;
using FellSky.Systems.SceneGraphRenderers;
using FellSky.Services;

namespace FellSky.Game.Ships.Parts
{
    public class NavLight: ShipPart
    {
        public float Amplitude { get; set; } = 1;
        public float Frequency { get; set; } = 1;
        public float PhaseShift { get; set; } = 0;
        public float VerticalShift { get; set; } = 0;

        public override Entity CreateEntity(EntityWorld world, Entity ship, Entity parent, int? index)
        {
            var entity = world.CreateEntity();
            ship.AddChild(entity, index);
            var thruster = new NavLightComponent(this, ship);
            entity.AddComponent<IShipPartComponent>(thruster);
            entity.AddComponent(thruster);
            entity.AddSceneGraphRendererComponent<StandardShipRenderer>();
            entity.AddComponent(Transform);
            var spriteManager = ServiceLocator.Instance.GetService<ISpriteManagerService>();
            var spriteComponent = spriteManager.CreateSpriteComponent(SpriteId);
            entity.AddComponent(spriteComponent);
            entity.AddComponent(new BoundingBoxComponent(new FloatRect(0, 0, spriteComponent.TextureRect.Width, spriteComponent.TextureRect.Height)));
            return entity;
        }
    }
}
