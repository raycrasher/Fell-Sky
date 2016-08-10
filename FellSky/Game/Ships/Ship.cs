using Artemis.Interface;
using FellSky.Framework;
using FellSky.Game.Ships.Modules;
using FellSky.Game.Ships.Parts;
using FellSky.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Artemis;
using FellSky.Components;
using FellSky.Systems.SceneGraphRenderers;
using FellSky.Systems;
using FellSky.EntityFactories;

namespace FellSky.Game.Ships
{
    [Archetype]
    public class Ship: IPersistable
    {
        public string Id { get; set; }
        public string GivenName { get; set; }
        public string HullClass { get; set; }
        public string ShipType { get; set; }

        public string ModelId { get; set; }


        [ExpandableObject]
        public ShipHandlingParameters Handling { get; set; } = new ShipHandlingParameters();

        public Color BaseDecalColor { get; set; } = Color.White;
        public Color TrimDecalColor { get; set; } = Color.White;

        public Entity CreateEntity(EntityWorld world, Vector2 position, float rotation, Vector2? scale=null, bool physics=true)
        {
            var shipEntity = world.CreateEntity();
            var shipComponent = new ShipComponent(this);
            shipEntity.AddComponent(new Transform(position, rotation, scale ?? Vector2.One));
            shipEntity.AddComponent(shipComponent);
            shipEntity.AddComponent(new SceneGraphComponent());
            shipEntity.AddComponent(new SceneGraphRenderRoot<StandardShipModelRenderer>());

            if (physics)
            {
                var rigidBody = world.SystemManager.GetSystem<PhysicsSystem>().CreateRigidBody(position, rotation);
                shipEntity.AddComponent(rigidBody);
                rigidBody.Body.IsStatic = false;
                rigidBody.Body.AngularDamping = Handling.AngularDamping;
                rigidBody.Body.LinearDamping = Handling.LinearDamping;
            }

            var model = ShipEntityFactory.GetShipModel(ModelId);
            model.CreateChildEntities(world, shipEntity);
            
            var partLookup = shipEntity.GetChildren()
                             .ToDictionary(c => c.GetComponent<IShipPartComponent>().Part, c => c);
            
            foreach (var hp in model.Hardpoints)
            {
                var hullEntity = partLookup[hp.Hull];
                hullEntity.AddComponent(new HardpointComponent(hp));
                shipComponent.Hardpoints.Add(hullEntity);
            }
            var shipModelComponent = new ShipModelComponent { Model = model };
            shipModelComponent.BaseDecalColor = BaseDecalColor;
            shipModelComponent.TrimDecalColor = TrimDecalColor;

            shipEntity.AddComponent(shipModelComponent);
            shipEntity.Refresh();


            return shipEntity;
        }

        /*
        public FloatRect CalculateBoundingBox(float thrusterXScale = 0.3f)
        {
            var sprites = ServiceLocator.Instance.GetService<ISpriteManagerService>().Sprites;
            return Parts.Aggregate(new FloatRect(), (box, part) =>
                {
                    Sprite spr;
                    if (!sprites.TryGetValue(part.SpriteId, out spr)) return box;

                    var aabb = new FloatRect (-(spr.OriginX ?? spr.W / 2), -(spr.OriginY ?? spr.H / 2), spr.W, spr.H);
                    
                    var topleft = new Vector2(Math.Min(aabb.Left, aabb.Right), Math.Min(aabb.Top, aabb.Bottom));
                    var bottomright = new Vector2(Math.Max(aabb.Left, aabb.Right), Math.Max(aabb.Top, aabb.Bottom));

                    var matrix = part.Transform.Matrix;
                    if(part is Thruster)
                    {
                        matrix = Matrix.CreateScale(new Vector3(thrusterXScale, 1, 1)) * matrix;
                    }

                    Vector2.Transform(ref topleft, ref matrix, out topleft);
                    Vector2.Transform(ref bottomright, ref matrix, out topleft);

                    return new FloatRect(new Vector2(Math.Min(Math.Min(topleft.X, bottomright.X), box.Left), Math.Min(Math.Min(topleft.Y, bottomright.Y), box.Top)),
                        new Vector2(Math.Max(Math.Abs(topleft.X - bottomright.X), box.Width), Math.Max(Math.Abs(topleft.Y - bottomright.Y), box.Width)));
                });
            
        }
        */
    }
}
