using Artemis;
using FellSky.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System;
using FellSky.Game.Ships.Parts;
using FellSky.Components;

namespace FellSky.Systems
{
    public class ShipRendererSystem : Artemis.System.EntitySystem
    {
        SpriteBatch _spriteBatch;
        CameraComponent _camera;
        private Matrix _matrix;

        public string CameraTag { get; set; }

        public ShipRendererSystem(SpriteBatch spriteBatch, string cameraTag)
            : base(Aspect.All(typeof(ShipComponent)))
        {
            CameraTag = cameraTag;
            _spriteBatch = spriteBatch;
        }

        protected override void Begin()
        {
            base.Begin();
            _camera = EntityWorld.TagManager.GetEntity(CameraTag)?.GetComponent<CameraComponent>();
        }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            if (_camera == null) return;
            _matrix = _camera.GetViewMatrix(1.0f);
            _spriteBatch.Begin(SpriteSortMode.Deferred, blendState: BlendState.AlphaBlend, transformMatrix: _matrix, samplerState: SamplerState.PointClamp);

            foreach (var entity in entities.Values)
            {
                DrawShip(_spriteBatch, entity);
            }

            _spriteBatch.End();
        }

        public static void DrawShip(SpriteBatch spriteBatch, Entity ship)
        {
            var shipComponent = ship.GetComponent<ShipComponent>();
            var xform = ship.GetComponent<Transform>();
            var shipMatrix = xform.Matrix;

            foreach(var item in shipComponent.PartEntities)
            {
                if (item.Entity.HasComponent<HullComponent>())
                    DrawHull(spriteBatch, ship, item.Entity, ref shipMatrix);
                if (item.Entity.HasComponent<ThrusterComponent>())
                    DrawThruster(spriteBatch, ship, item.Entity, ref shipMatrix);
            }
        }

        private static void DrawThruster(SpriteBatch spriteBatch, Entity ship, Entity thrusterEntity, ref Matrix shipMatrix)
        {            
            var shipComponent = ship.GetComponent<ShipComponent>();          
            var thrusterComponent = thrusterEntity.GetComponent<ThrusterComponent>();
            if (thrusterComponent.ThrustPercentage > 0)
            {
                var sprite = thrusterEntity.GetComponent<SpriteComponent>();
                var thruster = thrusterComponent.Part;
                var color = Color.Lerp(new Color(thruster.Color, 0), thruster.Color, MathHelper.Clamp(thrusterComponent.ThrustPercentage, 0, 1));
                sprite.Draw(batch: spriteBatch, matrix: thruster.Transform.Matrix * shipMatrix, color: thruster.Color);
            }
        }

        static Transform _tmpXform = new Transform();

        private static void DrawHull(SpriteBatch spriteBatch, Entity ship, Entity hullEntity, ref Matrix shipMatrix)
        {
            var xform = ship.GetComponent<Transform>();
            var shipComponent = ship.GetComponent<ShipComponent>();
            var basedecal = shipComponent.Ship.BaseDecalColor.ToVector4();
            var trimdecal = shipComponent.Ship.TrimDecalColor.ToVector4();

            var hullComponent = hullEntity.GetComponent<HullComponent>();
            var sprite = hullEntity.GetComponent<SpriteComponent>();
            var hull = hullComponent.Part;
            var color = hull.Color;
            switch (hull.ColorType)
            {
                case HullColorType.BaseDecal:
                    color = new Color(basedecal * color.ToVector4());
                    break;
                case HullColorType.TrimDecal:
                    color = new Color(trimdecal * color.ToVector4());
                    break;
            }

            _tmpXform.CopyValuesFrom(hull.Transform);

            SpriteEffects fx = SpriteEffects.None;
            if (_tmpXform.Scale.X < 0)
            {
                fx |= SpriteEffects.FlipHorizontally;
                _tmpXform.Scale *= new Vector2(-1, 1);

            }
            if (_tmpXform.Scale.Y < 0)
            {
                fx |= SpriteEffects.FlipVertically;
                _tmpXform.Scale *= new Vector2(1, -1);
            }

            sprite.Draw(batch: spriteBatch, matrix: _tmpXform.Matrix * shipMatrix, color:color, effects: fx);
        }
    }
}
