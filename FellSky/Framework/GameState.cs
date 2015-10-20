using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Framework
{
    public abstract class GameState
    {
        public virtual void Enter(GameState previous) { }
        public virtual void Exit(GameState next) { }
        public virtual void Pause() { }
        public virtual void Resume() { }
        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(GameTime gameTime) { }
        public virtual void LoadContent() { }
        public virtual void UnloadContent() { }
    }
}
