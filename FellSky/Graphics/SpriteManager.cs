using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Xna.Framework.Graphics;

namespace FellSky.Graphics
{
    public static class SpriteManager
    {
        struct JsonSpriteSheet
        {
            public string texture;
            public JsonSprite[] sprites;
        }

        struct JsonSprite
        {
            public string id;
            public int x, y, w, h;

            public Sprite GetSprite(Texture2D tex)
            {
                return new Sprite(id, tex, new Microsoft.Xna.Framework.Rectangle(x, y, w, h));
            }
        }

        public static Dictionary<string, Sprite> Sprites { get; } = new Dictionary<string, Sprite>();

        public static void AddSpriteSheetFromFile(string filename)
        {
            var sheet = JsonConvert.DeserializeObject<JsonSpriteSheet>(System.IO.File.ReadAllText(filename));
            var tex = Game.Instance.Content.Load<Texture2D>(sheet.texture);
            foreach(var s in sheet.sprites)
            {
                Sprites[s.id] = s.GetSprite(tex);
            }
        }
    }
}
