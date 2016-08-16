using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using FellSky.Components;

namespace FellSky.Game.Ships.Parts
{
    public class DummyPart : ShipPart
    {
        public bool IsWeaponMuzzle { get; set; }

        public override Entity CreateEntity(EntityWorld world, Entity ship, Entity parent, int? index = default(int?))
        {
            var entity = world.CreateEntity();
            parent.AddChild(entity);
            entity.AddComponent(Transform.Clone());
            var component = new DummyPartComponent(this, ship);
            entity.AddComponent(component);
            entity.AddComponent<IShipPartComponent>(component);
            entity.AddComponent(new BoundingBoxComponent(new FloatRect(-10, -10, 20, 20)));
            return entity;
        }
    }
}
