using FellSky.Common;
using FellSky.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

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
        public ShipSprite(XElement xmlNode)
        {
            SubSprites.AddRange(
                xmlNode.Elements("Sprite")
                .Select(x =>
                {
                    var item = new ShipSpriteItem();
                    item.SpriteId = x.Attribute("spriteid").Value;
                    item.Tag = x.Attribute("tag")?.Value;
                    item.Color = x.Attribute("color")?.Value.ToColorFromHexString() ?? Color.White;
                    item.Transform.Position = x.Attribute("position").Value.ToVector2();
                    item.Transform.Scale = x.Attribute("scale")?.Value.ToVector2() ?? Vector2.One;
                    item.Transform.Rotation = float.Parse(x.Attribute("rotation")?.Value ?? "0");
                    item.Transform.Origin = x.Attribute("origin")?.Value.ToVector2() ?? Vector2.Zero;
                    item.GlowColor = x.Attribute("glowcolor")?.Value.ToColorFromHexString();
                    item.ColorType = (ShipSpriteColorType) Enum.Parse(typeof(ShipSpriteColorType), x.Attribute("colortype")?.Value ?? "None");
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
