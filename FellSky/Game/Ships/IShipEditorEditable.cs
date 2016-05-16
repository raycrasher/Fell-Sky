using FellSky.Game.Ships.Parts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Ships
{
    public interface IShipEditorEditableModel
    {
        IList<ShipPart> Parts { get; }
        Color BaseDecalColor { get; set; }
        Color TrimDecalColor { get; set; }
        void SaveToJsonFile(string filename);
    }

    public interface IShipEditorEditableComponent
    {
        IShipEditorEditableModel Model { get; }
        IList<PartEntityPair> PartEntities { get; }
    }
}
