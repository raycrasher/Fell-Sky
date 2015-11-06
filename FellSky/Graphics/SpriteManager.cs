using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace FellSky.Graphics
{
    public static class SpriteManager
    {
        public static Dictionary<string, Sprite> Sprites { get; } = new Dictionary<string, Sprite>();

        public static JsonSpriteSheet AddSpriteSheetFromFile(ContentManager manager, string filename)
        {
            var sheet = JsonConvert.DeserializeObject<JsonSpriteSheet>(System.IO.File.ReadAllText(filename));
            var tex = manager.Load<Texture2D>(sheet.Texture);
            foreach(var s in sheet.Sprites)
            {
                Sprites[s.Id] = s.GetSprite(tex);
            }
            return sheet;
        }
    }

    public class JsonSpriteSheet
    {
        public string Texture { get; set; }
        public JsonSprite[] Sprites { get; set; }
    }

    public class JsonSprite
    {
        public string Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int W { get; set; }
        public int H { get; set; }
        public float? OriginX { get; set; }
        public float? OriginY { get; set; }
        public float? Padding { get; set; }
        public string Type { get; set; }
        public string Subtype { get; set; }
        public string Tags { get; set; }
        public List<JsonSprite> Subsprites { get; set; }

        public Sprite GetSprite(Texture2D tex)
        {
            return new Sprite(Id, tex, new Microsoft.Xna.Framework.Rectangle(X, Y, W, H));
        }
    }
}
