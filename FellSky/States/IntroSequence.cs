using FellSky.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace FellSky.States { 
    public class IntroSequence: GameState
    {
        public enum State
        {
            Start,
            Intro,
            Launch,
        }

        public bool SequenceDone { get; private set; }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public System.Collections.IEnumerable DoIntroSequence()
        {
            throw new NotImplementedException();
        }
    }
}
