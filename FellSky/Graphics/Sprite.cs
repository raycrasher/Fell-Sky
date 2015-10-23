using Artemis.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FellSky.Graphics
{
    public class Sprite : IComponent
    {
        public string Name { get; set; }
        public Texture2D Texture { get; set; }
        public Rectangle TextureRect { get; set; }

        public Sprite()
        {
        }

        public Sprite(Texture2D texture)
        {
            Texture = texture;
            TextureRect = new Rectangle(0, 0, texture.Width, texture.Height);
        }

        public Sprite(Texture2D texture, Rectangle textureRect)
        {
            Texture = texture;
            TextureRect = textureRect;
        }

        public Sprite(string name, Texture2D texture)
            : this(texture)
        {
            Name = name;
            Texture = texture;
            TextureRect = new Rectangle(0, 0, texture.Width, texture.Height);
        }

        public Sprite(string name, Texture2D texture, Rectangle textureRect)
        {
            Name = name;
            Texture = texture;
            TextureRect = textureRect;
        }


        public Sprite Clone()
        {
            return new Sprite(Name, Texture, TextureRect);
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
