using Artemis.Interface;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace FellSky.Game.Ships.Parts
{
    public abstract class ShipPart: ICloneable
    {
        public string Name { get; set; }
        [Browsable(true), TypeConverter(typeof(ExpandableObjectConverter))]
        public Transform Transform { get; set; } = new Transform();
        public string SpriteId { get; set; }
        [Browsable(true), TypeConverter(typeof(ExpandableObjectConverter))]
        public Color Color { get; set; }

        public HashSet<string> Flags { get; set; } = new HashSet<string>();

        public ShipPart()
        {
             Name = Name ?? this.GetType().Name;
        }

        object ICloneable.Clone() => Clone();

        public virtual ShipPart Clone()
        {
            var part = (ShipPart) MemberwiseClone();
            part.Transform = Transform.Clone();
            return part;
        }

        public override string ToString()
        {
            return Name ?? $"* {base.ToString()}";
        }

        public abstract Entity CreateEntity(EntityWorld world, Entity ship, Entity parent, int? index = null);
        public Entity CreateEntity(EntityWorld world, Entity ship, int? index=null)
        {
            return CreateEntity(world, ship, ship, index);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            Flags = Flags ?? new HashSet<string>();
        }
    }
}
