using Artemis.Interface;
using FellSky.Game.Ships.Parts;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Ships
{
    public class Ship
    {
        public string GivenName { get; set; }
        public string HullClass { get; set; }
        public string ShipType { get; set; }

        public List<ShipPart> Parts { get; set; } = new List<ShipPart>();
        public ShipHandlingParameters Handling { get; set; } = new ShipHandlingParameters();

        public Color BaseDecalColor { get; set; } = Color.White;
        public Color TrimDecalColor { get; set; } = Color.White;

        public void SaveToJsonFile(string filename)
        {
            System.IO.File.WriteAllText( filename, JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            }));
        }

        public static Ship LoadFromJsonFile(string filename)
        {
            return JsonConvert.DeserializeObject<Ship>(System.IO.File.ReadAllText(filename), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
        }
    }
}
