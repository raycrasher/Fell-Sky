using Artemis.Interface;
using FellSky.Game.Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public class ShipPartGroupComponent : IComponent, IShipEditorEditableComponent
    {
        public ShipPartGroupComponent(ShipPartGroup group)
        {
            PartGroup = group;
        }

        public ShipPartGroup PartGroup { get; set; }

        IShipEditorEditableModel IShipEditorEditableComponent.Model => PartGroup;

        public IList<PartEntityPair> PartEntities { get; set; } = new List<PartEntityPair>();
    }
}
