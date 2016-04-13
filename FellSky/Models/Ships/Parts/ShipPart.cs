using Artemis.Interface;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Models.Ships.Parts
{
    public abstract class ShipPart: ICloneable
    {
        public string Name { get; set; }
        public PartGroup Group { get; set; }
        [Xceed.Wpf.Toolkit.PropertyGrid.Attributes.ExpandableObject]
        public Transform Transform { get; set; } = new Transform();
        public string SpriteId { get; set; }
        public Color Color { get; set; }
        public float Depth { get; set; } = 0.5f;

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
    }
}
