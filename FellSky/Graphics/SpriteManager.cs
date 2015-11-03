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
            var tex = manager.Load<Texture2D>(sheet.texture);
            foreach(var s in sheet.sprites)
            {
                Sprites[s.id] = s.GetSprite(tex);
            }
            return sheet;
        }
    }

    public class JsonSpriteSheet
    {
        public string texture { get; set; }
        public JsonSprite[] sprites { get; set; }
    }

    public class JsonSprite
    {
        public string id { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int w { get; set; }
        public int h { get; set; }
        public float? origin_x { get; set; }
        public float? origin_y { get; set; }
        public float? padding { get; set; }
        public string type { get; set; }
        public string subtype { get; set; }
        public string tags { get; set; }
        public JsonSprite[] subsprites { get; set; }

        public Sprite GetSprite(Texture2D tex)
        {
            return new Sprite(id, tex, new Microsoft.Xna.Framework.Rectangle(x, y, w, h));
        }
    }
}
