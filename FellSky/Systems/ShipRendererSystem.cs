﻿using Artemis;
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
                var xform = ship.GetComponent<Transform>();
                var shipMatrix = xform.Matrix;
                var shipComponent = ship.GetComponent<ShipComponent>();

                foreach(var entity in shipComponent.ThrusterEntities)
                {
                    var thrusterComponent = entity.GetComponent<ThrusterComponent>();
                    if (thrusterComponent.ThrustPercentage > 0)
                    {
                        var sprite = entity.GetComponent<SpriteComponent>();
                        var thruster = thrusterComponent.Part;
                        var color = Color.Lerp(new Color(thruster.Color, 0), thruster.Color, MathHelper.Clamp(thrusterComponent.ThrustPercentage, 0, 1));
                        sprite.Draw(batch: _spriteBatch, matrix: thruster.Transform.Matrix * shipMatrix, color: thruster.Color, depth: thruster.Depth);
                    }
                }
            }
        }

        private void DrawHulls(ICollection<Entity> entities)
        {
            Transform tmpXform = new Transform();
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

                    tmpXform.CopyValuesFrom(hull.Transform);

                    SpriteEffects fx = SpriteEffects.None;
                    if (tmpXform.Scale.X < 0)
                    {
                        fx |= SpriteEffects.FlipHorizontally;
                        tmpXform.Scale *= new Vector2(-1, 1);

                    }
                    if (tmpXform.Scale.Y < 0)
                    {
                        fx |= SpriteEffects.FlipVertically;
                        tmpXform.Scale *= new Vector2(1, -1);
                    }

                    sprite.Draw(batch: _spriteBatch, matrix: tmpXform.Matrix * shipMatrix, color:color, depth: hull.Depth, effects: fx);
                }
            }
        }
    }
}
