using Artemis;
using FellSky.Components;
using FellSky.Game.Crew;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Campaign.Storyline
{
    public static class StoryUtilities
    {
        public static Entity CreateStoryOverlay(this EntityWorld world, string text, string className = "storyoverlay")
        {
            var entity = world.CreateEntity();
            var element = new LibRocketNet.Element("div");
            element.SetClass(className);
            element.InnerRml = text;
            entity.AddComponent(new GuiComponent(element));
            return entity;
        }

        public static void Speak(Character character, string text)
        {

        }
    }
}
