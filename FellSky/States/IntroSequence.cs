using FellSky.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public System.Collections.IEnumerable DoIntroSequence()
        {
            throw new NotImplementedException();
        }
    }
}
