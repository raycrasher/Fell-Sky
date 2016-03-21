using FellSky.Framework.ShapeDefinitions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Services
{
    public interface IShapeManagerService
    {
        void LoadShapes(string filename);
        void SaveShapes(string filename);
        ShapeDefinition GetShape(string shapeId);
    }

    public class ShapeManagerService : IShapeManagerService
    {
        private Dictionary<string, ShapeDefinition> _shapes = new Dictionary<string, ShapeDefinition>();

        public ShapeManagerService()
        {
            foreach (var file in Properties.Settings.Default.LoadedShapeLists)
                LoadShapes(file);
        }

        public void LoadShapes(string filename)
        {
            if (System.IO.File.Exists(filename))
            {
                var items = JsonConvert.DeserializeObject<Dictionary<string, ShapeDefinition>>(System.IO.File.ReadAllText(filename), new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                foreach(var item in items)
                {
                    _shapes[item.Key] = item.Value;
                }
            }
        }

        public void SaveShapes(string filename)
        {
            System.IO.File.WriteAllText(filename, JsonConvert.SerializeObject(_shapes, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            }));
        }

        public ShapeDefinition GetShape(string shapeId) => _shapes[shapeId];

    }
}
