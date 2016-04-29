using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Campaign
{
    public enum MainStoryProgress
    {
        Intro,
        ActOne,
        ActTwo,
        ActThree
    }

    public class Story
    {
        public MainStoryProgress Progress;

        public void UpdateStory()
        {

        }
    }
}
