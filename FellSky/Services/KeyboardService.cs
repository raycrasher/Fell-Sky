using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FellSky.Services
{
    public class KeyboardService: IKeyboardService
    {
        private HashSet<Keys> _downKeys = new HashSet<Keys>();

        public static KeyboardService Instance { get; set; }

        public KeyboardService(CoroutineService coroutineService)
        {
            //_lastKeyboardState = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            coroutineService.StartCoroutine(Update());

            Instance = this;
        }

        public event Action<Keys> KeyDown, KeyUp;

        private System.Collections.IEnumerable Update()
        {
            while (true)
            {

                var pressedKeys = Keyboard.GetState().GetPressedKeys();
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
