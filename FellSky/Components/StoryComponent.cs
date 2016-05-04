using Artemis.Interface;
using FellSky.Game.Campaign.Storyline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public class StoryComponent: IComponent
    {
        public IStoryAct Act;
        public string State;
    }
}
