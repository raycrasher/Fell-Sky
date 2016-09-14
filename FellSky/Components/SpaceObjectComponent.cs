using Artemis.Interface;
using FellSky.Game.Space;
using Microsoft.Xna.Framework.Graphics;
using FellSky.Services;
using Microsoft.Xna.Framework.Content;

namespace FellSky.Components
{
    public class SpaceObjectComponent: IComponent
    {
        public Texture2D Texture;

        public SpaceObject Object { get; private set; }

        public SpaceObjectComponent(SpaceObject obj)
        {
            Object = obj;
            if (obj.TextureId != null)
            {
                //Texture = ServiceLocator.Instance.GetService<ContentManager>().Load<Texture2D>(obj.TextureId);
            }
        }
    }
}
