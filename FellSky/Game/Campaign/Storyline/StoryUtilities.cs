using Artemis;
using FellSky.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Campaign.Storyline
{
    public static class StoryUtilities
    {
        public static Entity CreateStoryOverlay(this EntityWorld world, string text)
        {
            var entity = world.CreateEntity();
            var element = new LibRocketNet.Element("div");
            element.SetClass("storyoverlay");
            element.InnerRml = text;
            entity.AddComponent(new GuiComponent { Element = element });
            return entity;
        }
    }
}
