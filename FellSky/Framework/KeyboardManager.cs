using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FellSky.Framework
{
    public class KeyboardManager
    {
        private HashSet<Keys> _downKeys = new HashSet<Keys>();

        public KeyboardManager(CoroutineManager manager)
        {
            //_lastKeyboardState = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            manager.StartCoroutine(Update());
        }

        public event Action<Keys> KeyDown, KeyUp;

        private System.Collections.IEnumerable Update()
        {
            while (true)
            {

                var pressedKeys = Microsoft.Xna.Framework.Input.Keyboard.GetState().GetPressedKeys();
                // check for keydown
                foreach (var k in pressedKeys)
                {
                    if (!_downKeys.Contains(k))
                    {
                        _downKeys.Add(k);
                        if (KeyDown != null) KeyDown(k);
                    }
                }
                var dk = _downKeys.ToArray();
                foreach (var k in dk)
                {
                    if (!pressedKeys.Contains(k))
                    {
                        _downKeys.Remove(k);
                        if (KeyUp != null) KeyUp(k);
                    }
                }
                yield return null;
            }
        }

        public bool IsKeyDown(Keys UpKey)
        {
            return _downKeys.Contains(UpKey);
        }
    }
}
