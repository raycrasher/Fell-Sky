using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Scenes
{
    public abstract class Scene: IDisposable
    {
        public virtual void Enter(Scene previous) { }
        public virtual void Exit(Scene next) { }
        public virtual void Pause() { }
        public virtual void Resume() { }
        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(GameTime gameTime) { }
        public virtual void LoadContent() { }
        public virtual void UnloadContent() { }

        bool _isDisposed = false;

        public void Dispose()
        {
            if (!_isDisposed)
            {
                UnloadContent();
                _isDisposed = true;
            }
        }
    }
}
