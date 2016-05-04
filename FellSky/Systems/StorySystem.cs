using FellSky.Game.Campaign.Storyline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Systems
{
    class StorySystem : Artemis.System.ProcessingSystem
    {
        public Story Story { get; private set; }
        public override void LoadContent()
        {
            Story = new Story(EntityWorld);
            base.LoadContent();
        }

        public override void ProcessSystem()
        {
            
        }
    }
}
