using FellSky.Common;
using FellSky.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Artemis.Interface;
using Microsoft.Xna.Framework.Graphics;

namespace FellSky.Graphics
{
    public enum ShipSpriteColorType
    {
        None=0, Base=1, Trim=2
    }

    public class SpriteGroup
    {
        private Color _baseColor;
        private Color _trimColor;

        public List<SubSprite> SubSprites { get; set; } = new List<SubSprite>();
        public SpriteGroup() { }

        // load from ShipEditor generated xml.
        public SpriteGroup(JObject json)
        {
            SubSprites.AddRange(
                json["sprites"].Select(token =>
                {
                    if (token["_type"]?.Value<string>() != typeof(SubSprite).Name)
                        throw new InvalidOperationException("ShipSpriteItem cannot load data from JSon; data is invalid.");

                    var item = new SubSprite();
                    item.SpriteId = token["spriteid"]?.Value<string>();
                    item.Tag = token["tag"]?.Value<string>();
                    item.Color = token["color"]?.Value<string>().ToColorFromHexString() ?? Color.White;
                    item.Transform.Position = token["position"]?.Value<string>().ToVector2() ?? Vector2.Zero;
                    item.Transform.Scale = token["scale"]?.Value<string>().ToVector2() ?? Vector2.One;
                    item.Transform.Rotation = token["rotation"]?.Value<float>() ?? 0f;
                    item.Transform.Origin = token["origin"]?.Value<string>().ToVector2() ?? Vector2.Zero;
                    item.GlowColor = token["glowcolor"]?.Value<string>().ToColorFromHexString();
                    item.ColorType = token["colortype"]?.Value<ShipSpriteColorType>() ?? ShipSpriteColorType.None;
                    item.Sprite = SpriteManager.Sprites[item.SpriteId];
                    return item;
                }
                ));
        }

        /// <summary>
        /// Creates a clone of the sprite.
        /// </summary>
        /// <returns></returns>
        public SpriteGroup Clone() {
            var group = new SpriteGroup();
            group.SubSprites = new List<SubSprite>(SubSprites.Select(s => s.Clone()));
            return group;
        }

        /// <summary>
        /// Base color
        /// </summary>
        public Color BaseColor {
            get { return _baseColor; }
            set
            {
                if(value!=_baseColor)
                    SetColor(value, ShipSpriteColorType.Base);
                _baseColor = value;

            }
        }

        /// <summary>
        /// Trim color
        /// </summary>
        public Color TrimColor
        {
            get { return _trimColor; }
            set
            {
                if(value!=_trimColor)
                    SetColor(value, ShipSpriteColorType.Trim);
                _trimColor = value;
            }
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

        public void Draw(SpriteBatch batch, ref Matrix parentMatrix)
        {
            for(int i = 0; i < SubSprites.Count; i++)
            {
                var sprite = SubSprites[i];
                var matrix = parentMatrix * sprite.Transform.Matrix;
                sprite.Draw(batch, ref matrix);
            }
        }
    }

    public delegate void ShipSpriteDrawFunction(SpriteBatch batch, ref Matrix matrix, SubSprite item);

    public class SubSprite
    {
        public Transform Transform { get; set; } = new Transform();
        public string SpriteId { get; set; }
        public Sprite Sprite { get; set; }
        public Color Color { get; set; } = Color.White;
        public Color? GlowColor { get; set; } = null;
        public ShipSpriteColorType ColorType { get; set; } = ShipSpriteColorType.None;
        public string Tag { get; set; }
        public SpriteEffects SpriteEffect { get; set; } = SpriteEffects.None;

        public SubSprite Clone()
        {
            var item = (SubSprite) MemberwiseClone();
            item.Transform = Transform.Clone();
            return item;
        }

        public void Draw(SpriteBatch batch, ref Matrix matrix)
        {
            Vector2 position, scale;
            float rotation;
            Utilities.DecomposeMatrix2D(ref matrix, out position, out rotation, out scale);
            Sprite.Draw(batch, position, rotation, scale, Vector2.Zero, Color, SpriteEffect);
            OnDraw?.Invoke(batch, ref matrix, this);
        }

        public event ShipSpriteDrawFunction OnDraw;
    }
}
