﻿using Artemis.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FellSky.Framework;
using Microsoft.Xna.Framework.Content;

namespace FellSky.Components
{
    public class SpriteComponent : IComponent
    {
        public string Name { get; set; }
        public Texture2D Texture { get; set; }
        public Rectangle TextureRect { get; set; }
        public Vector2 Origin { get; set; }

        public SpriteComponent()
        {
        }

        public SpriteComponent(Sprite sprite, Texture2D texture)
        {
            Name = sprite.Id;
            Texture = texture;
            Origin = new Vector2(sprite.OriginX ?? sprite.W/2, sprite.OriginY ?? sprite.H/2);
            TextureRect = new Rectangle(sprite.X, sprite.Y, sprite.W, sprite.H);
        }

        public SpriteComponent(Texture2D texture)
        {
            Texture = texture;
            TextureRect = new Rectangle(0, 0, texture.Width, texture.Height);
        }

        public SpriteComponent(Texture2D texture, Rectangle textureRect)
        {
            Texture = texture;
            TextureRect = textureRect;
        }

        public SpriteComponent(string name, Texture2D texture)
            : this(texture)
        {
            Name = name;
            Texture = texture;
            TextureRect = new Rectangle(0, 0, texture.Width, texture.Height);
        }

        public SpriteComponent(string name, Texture2D texture, Rectangle textureRect)
        {
            Name = name;
            Texture = texture;
            TextureRect = textureRect;
        }


        public SpriteComponent Clone()
        {
            return new SpriteComponent(Name, Texture, TextureRect);
        }

        public void Draw(SpriteBatch batch, Vector2 position)
        {
            batch.Draw(Texture, position, TextureRect);
        }

        public void Draw(SpriteBatch batch, Vector2 position, float rotation)
        {
            batch.Draw(Texture, position, TextureRect, Color.White, rotation, new Vector2(TextureRect.Width / 2, TextureRect.Height / 2), Vector2.One, SpriteEffects.None, 0);
        }

        public void Draw(SpriteBatch batch, Vector2 position, float rotation, Vector2 scale)
        {
            batch.Draw(Texture, position, TextureRect, Color.White, rotation, new Vector2(TextureRect.Width / 2, TextureRect.Height / 2), scale, SpriteEffects.None, 0);
        }

        public void Draw(SpriteBatch batch, Vector2 position, float rotation, Vector2 scale, SpriteEffects effects = SpriteEffects.None, float depth = 0)
        {
            batch.Draw(Texture, position, TextureRect, Color.White, rotation, new Vector2(TextureRect.Width / 2, TextureRect.Height / 2), scale, SpriteEffects.None, 0);
        }

        public void Draw(SpriteBatch batch, Matrix matrix, Color color, SpriteEffects effects = SpriteEffects.None, float depth = 0)
        {
            Vector2 position, scale;
            float rotation;
            Utilities.DecomposeMatrix2D(ref matrix, out position, out rotation, out scale);
            Draw(batch, position, rotation, scale, Vector2.Zero, color, effects, depth);
        }

        public void Draw(SpriteBatch batch, Matrix matrix, Vector2 origin, Color color, float depth, SpriteEffects effects)
        {
            Vector2 position, scale;
            float rotation;
            Utilities.DecomposeMatrix2D(ref matrix, out position, out rotation, out scale);
            Draw(batch, position, rotation, scale, origin, color, effects, depth);
        }

        public void Draw(SpriteBatch batch, Vector2 position, float rotation, Vector2 scale, Vector2 origin, SpriteEffects effects = SpriteEffects.None, float depth = 0)
        {
            batch.Draw(Texture, position, TextureRect, Color.White, rotation, origin, scale, effects, depth);
        }

        public void Draw(SpriteBatch batch, Vector2 position, float rotation, Vector2 scale, Vector2 origin, Color color, SpriteEffects effects = SpriteEffects.None, float depth = 0)
        {
            batch.Draw(Texture, position, TextureRect, color, rotation, origin, scale, effects, depth);
        }

        public void Draw(SpriteBatch batch, ITransform transform, SpriteEffects effects = SpriteEffects.None, float depth = 0)
        {
            batch.Draw(Texture, transform.Position, TextureRect, Color.White, transform.Rotation, transform.Origin, transform.Scale, effects, depth);
        }

        public void Draw(SpriteBatch batch, ITransform transform, Color color, SpriteEffects effects = SpriteEffects.None, float depth = 0)
        {
            batch.Draw(Texture, transform.Position, TextureRect, color, transform.Rotation, transform.Origin, transform.Scale, effects, depth);
        }
    }
}
