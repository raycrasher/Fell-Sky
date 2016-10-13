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
            var iff = new IdFriendOrFoeComponent();
            var xform = shipEntity.AddComponentFromPool<Transform>();
            xform.Position = position;
            xform.Rotation = rotation;
            xform.Scale = scale ?? Vector2.One;

            shipEntity.AddComponent(shipComponent);
            shipEntity.AddComponent(new SceneGraphComponent());
            shipEntity.AddComponent(new SceneGraphRenderRoot<StandardShipModelRenderer>());
            shipEntity.AddComponent(iff);

            if (physics)
            {
                var rigidBody = world.SystemManager.GetSystem<PhysicsSystem>().CreateRigidBody(position, rotation);
                shipEntity.AddComponent(rigidBody);
                rigidBody.Body.IsStatic = false;
                rigidBody.Body.AngularDamping = Handling.AngularDamping;
                rigidBody.Body.LinearDamping = Handling.LinearDamping;
            }

            var model = ShipEntityFactory.GetShipModel(ModelId);

            shipComponent.ShipModel = model;
            shipComponent.BoundingBox = model.CalculateBoundingBox();
            model.CreateChildEntities(world, shipEntity);
            foreach(var entity in shipEntity.GetChildren())
            {
                entity.AddComponent(iff);
            }

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
            shipEntity.AddComponent(new HealthComponent(model.Parts.OfType<Hull>().Aggregate(0f, (health, hull) => health + hull.Health)));
            shipEntity.AddComponent(new PowerComponent { StoredPower = 50, MaxPower = 100 });
            shipEntity.AddComponent(new HeatComponent { StoredHeat = 0, MaxHeat = 100 });

            return shipEntity;
        }       
    }
}
