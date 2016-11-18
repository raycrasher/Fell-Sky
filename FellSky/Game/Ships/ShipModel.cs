using Artemis;
using FellSky.Components;
using FellSky.Framework;
using FellSky.Game.Ships.Parts;
using FellSky.Services;
using FellSky.Systems.SceneGraphRenderers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Ships
{
    public class ShipModel: IPersistable
    {
        public string Id { get; set; }
        public List<ShipPart> Parts { get; set; } = new List<ShipPart>();
        public List<Hardpoint> Hardpoints { get; set; } = new List<Hardpoint>();

        public void CreateChildEntities(EntityWorld world, Entity parentEntity)
        {
            var modelComponent = new ShipModelComponent { Model = this };
            parentEntity.AddComponent(modelComponent);
            foreach (var part in Parts)
            {
                part.CreateEntity(world, parentEntity);
            }
            modelComponent.BoundingBox = CalculateBoundingBox(0);
        }

        public Entity CreateStandAloneEntity(EntityWorld world)
        {
            var entity = world.CreateEntity();
            entity.AddComponentFromPool<Transform>();
            entity.AddComponent(new SceneGraphComponent());
            entity.AddComponent(new SceneGraphRenderRoot<StandardShipModelRenderer>());
            CreateChildEntities(world, entity);
            
            return entity;
        }

        public FloatRect CalculateBoundingBox(float thrusterXScale = 0.3f)
        {
            var sprites = ServiceLocator.Instance.GetService<ISpriteManagerService>().Sprites;
            return Parts.Aggregate(new FloatRect(), (box, part) =>
            {
                Sprite spr;
                if (!sprites.TryGetValue(part.SpriteId, out spr)) return box;

                var aabb = new FloatRect(-(spr.OriginX ?? spr.W / 2), -(spr.OriginY ?? spr.H / 2), spr.W, spr.H);

                var topleft = new Vector2(Math.Min(aabb.Left, aabb.Right), Math.Min(aabb.Top, aabb.Bottom));
                var bottomright = new Vector2(Math.Max(aabb.Left, aabb.Right), Math.Max(aabb.Top, aabb.Bottom));

                var matrix = part.Transform.Matrix;
                if (part is Thruster)
                {
                    matrix = Matrix.CreateScale(new Vector3(thrusterXScale, 1, 1)) * matrix;
                }

                Vector2.Transform(ref topleft, ref matrix, out topleft);
                Vector2.Transform(ref bottomright, ref matrix, out topleft);

                return new FloatRect(new Vector2(Math.Min(Math.Min(topleft.X, bottomright.X), box.Left), Math.Min(Math.Min(topleft.Y, bottomright.Y), box.Top)),
                    new Vector2(Math.Max(Math.Abs(topleft.X - bottomright.X), box.Width), Math.Max(Math.Abs(topleft.Y - bottomright.Y), box.Width)));
            });

        }
    }
}
