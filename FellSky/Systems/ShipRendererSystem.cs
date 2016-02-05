using Artemis;
using FellSky.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System;
using FellSky.Models.Ships.Parts;
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
            _spriteBatch.Begin(SpriteSortMode.BackToFront, blendState: BlendState.AlphaBlend, transformMatrix: _matrix);
            DrawThrusters(entities.Values);
            DrawHulls(entities.Values);
            _spriteBatch.End();
        }

        private void DrawThrusters(ICollection<Entity> entities)
        {
            foreach (var ship in entities)
            {
                var shipComponent = ship.GetComponent<ShipComponent>();
                var xform = ship.GetComponent<Transform>();
                var shipMatrix = xform.Matrix;

                foreach(var thrusterComponent in shipComponent.ThrusterEntities.Select(t=>t.GetComponent<ThrusterComponent>()).Where(s=>s.ThrustPercentage>0))
                {
                    var thruster = thrusterComponent.Part;
                    var color = Color.Lerp(new Color(thruster.Color, 0), thruster.Color, MathHelper.Clamp(thrusterComponent.ThrustPercentage, 0, 1));
                    thrusterComponent.Sprite.Draw(batch: _spriteBatch, matrix: thruster.Transform.Matrix * shipMatrix, color: thruster.Color, depth: thruster.Depth);
                }
            }
        }

        private void DrawHulls(ICollection<Entity> entities)
        {
            
            foreach (var ship in entities)
            {
                var xform = ship.GetComponent<Transform>();
                var shipMatrix = xform.Matrix;
                var shipComponent = ship.GetComponent<ShipComponent>();
                var basedecal = shipComponent.Ship.BaseDecalColor.ToVector4();
                var trimdecal = shipComponent.Ship.TrimDecalColor.ToVector4();

                foreach (var hullEntity in shipComponent.HullEntities)
                {
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
                    sprite.Draw(batch: _spriteBatch, matrix: hull.Transform.Matrix * shipMatrix, color:color, depth: hull.Depth, effects: hull.SpriteEffect);
                }
            }
        }
    }
}
