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
        public Transform Transform { get; set; } = new Transform();
        public string SpriteId { get; set; }        
        public Color Color { get; set; }
        public HashSet<string> Flags { get; set; } = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

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
        protected void OnDeserialized(StreamingContext context)
        {
            Transform = Transform ?? new Transform();
            Flags = new HashSet<string>(Flags, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
