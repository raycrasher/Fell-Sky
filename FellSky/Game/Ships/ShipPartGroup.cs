using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FellSky.Game.Ships.Parts;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace FellSky.Game.Ships
{
    public class ShipPartGroup : IShipEditorEditableModel
    {
        public string Id { get; set; }
        public IList<ShipPart> Parts { get; set; } = new List<ShipPart>();
        public Color BaseDecalColor { get; set; }
        public Color TrimDecalColor { get; set; }
    }
}
