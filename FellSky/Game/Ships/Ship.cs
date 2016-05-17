using Artemis.Interface;
using FellSky.Game.Ships.Modules;
using FellSky.Game.Ships.Parts;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace FellSky.Game.Ships
{
    public class Ship: IShipEditorEditableModel
    {
        public string GivenName { get; set; }
        public string HullClass { get; set; }
        public string ShipType { get; set; }

        public List<ShipPart> Parts { get; set; } = new List<ShipPart>();
        public List<Hardpoint> Hardpoints { get; set; } = new List<Hardpoint>();
        public List<ModuleSlot> ModuleSlots { get; set; } = new List<ModuleSlot>();

        IList<ShipPart> IShipEditorEditableModel.Parts => Parts;

        [ExpandableObject]
        public ShipHandlingParameters Handling { get; set; } = new ShipHandlingParameters();

        public Color BaseDecalColor { get; set; } = Color.White;
        public Color TrimDecalColor { get; set; } = Color.White;
        
        public void SaveToJsonFile(string filename)
        {
            System.IO.File.WriteAllText( filename, JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                PreserveReferencesHandling = PreserveReferencesHandling.All
            }));
        }

        public static Ship LoadFromJsonFile(string filename)
        {
            return JsonConvert.DeserializeObject<Ship>(System.IO.File.ReadAllText(filename), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                PreserveReferencesHandling = PreserveReferencesHandling.All
            });
        }
    }
}
