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

        public class JsonSpriteSheet
        {
            public string texture = null;
            public JsonSprite[] sprites = null;
        }

        public class JsonSprite
        {
            public string id = null;
            public int x=0, y=0, w=0, h=0;
            public float origin_x = 0, origin_y=0;
            public float padding;
            public string type, subtype, tags;
            public JsonSprite[] subsprites;

            public Sprite GetSprite(Texture2D tex)
            {
                return new Sprite(id, tex, new Microsoft.Xna.Framework.Rectangle(x, y, w, h));
            }
        }

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
}
