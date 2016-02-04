using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using FellSky.Framework;
using FellSky.Components;

namespace FellSky.Services
{
    public interface ISpriteManagerService
    {
        Dictionary<string, Sprite> Sprites { get; }
        SpriteSheet AddSpriteSheetFromFile(ContentManager manager, string filename);
        SpriteComponent CreateSpriteComponent(string spriteId);
    }
}