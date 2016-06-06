namespace FellSky.Framework
{
    public class SpriteSheet: IPersistable
    {
        public string Texture { get; set; }
        public Sprite[] Sprites { get; set; }
    }
}
