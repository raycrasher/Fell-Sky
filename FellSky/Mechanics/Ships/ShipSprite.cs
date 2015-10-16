using FellSky.Common;
using FellSky.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FellSky.Mechanics.Ships
{
    public enum ShipSpriteColorType
    {
        None=0, Base=1, Trim=2
    }

    public class ShipSprite
    {
        public List<ShipSpriteItem> SubSprites { get; set; } = new List<ShipSpriteItem>();
        public ShipSprite() { }

        // load from ShipEditor generated xml.
        public ShipSprite(JObject json)
        {
            SubSprites.AddRange(
                json["sprites"].Select(token =>
                {
                    if (token["_type"]?.Value<string>() != typeof(ShipSpriteItem).Name)
                        throw new InvalidOperationException("ShipSpriteItem cannot load data from JSon; data is invalid.");

                    var item = new ShipSpriteItem();
                    item.SpriteId = token["spriteid"]?.Value<string>();
                    item.Tag = token["tag"]?.Value<string>();
                    item.Color = token["color"]?.Value<string>().ToColorFromHexString() ?? Color.White;
                    item.Transform.Position = token["position"]?.Value<string>().ToVector2() ?? Vector2.Zero;
                    item.Transform.Scale = token["scale"]?.Value<string>().ToVector2() ?? Vector2.One;
                    item.Transform.Rotation = token["rotation"]?.Value<float>() ?? 0f;
                    item.Transform.Origin = token["origin"]?.Value<string>().ToVector2() ?? Vector2.Zero;
                    item.GlowColor = token["glowcolor"]?.Value<string>().ToColorFromHexString();
                    item.ColorType = token["colortype"]?.Value<ShipSpriteColorType>() ?? ShipSpriteColorType.None;
                    return item;
                }
                ));
        }

        public ShipSprite Clone() {
            var group = new ShipSprite();
            group.SubSprites = new List<ShipSpriteItem>(SubSprites.Select(s => s.Clone()));
            return group;
        }

        public void Append(ShipSprite sprite)
        {
            SubSprites.AddRange(sprite.SubSprites.Select(s => s.Clone()));
        }

        public void AppendWithTransform(ShipSprite sprite, Transform transform)
        {
            SubSprites.AddRange(sprite.SubSprites.Select(s => {
                var item = s.Clone();
                item.Transform.Combine(transform);
                return item;
            }));
        }

        public void SetBaseColor(Color c)
        {
            SetColor(c, ShipSpriteColorType.Base);
        }
        
        public void SetTrimColor(Color c)
        {
            SetColor(c, ShipSpriteColorType.Trim);
        }

        public void SetColor(Color c, ShipSpriteColorType type)
        {
            var baseColorHsl = c.ToHSL();
            foreach (var sprite in SubSprites.Where(i => i.ColorType == type))
            {
                var hsl = sprite.Color.ToHSL();
                var newColorHsl = new ColorHSL(baseColorHsl.Hue, baseColorHsl.Saturation, hsl.Luminosity + baseColorHsl.Luminosity / 2);
                sprite.Color = newColorHsl.ToRgb();
            }
        }

    }

    public class ShipSpriteItem
    {
        public Transform Transform { get; set; } = new Transform();
        public string SpriteId { get; set; }
        public Sprite Sprite { get; set; }
        public Color Color { get; set; } = Color.White;
        public Color? GlowColor { get; set; } = null;
        public ShipSpriteColorType ColorType { get; set; } = ShipSpriteColorType.None;
        public string Tag { get; set; }

        public ShipSpriteItem Clone()
        {
            var item = (ShipSpriteItem) MemberwiseClone();
            item.Transform = Transform.Clone();
            return item;
        }
    }
}
