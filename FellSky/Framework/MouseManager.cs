using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;

namespace FellSky.Framework
{
    public class MouseManager
    {
        public event Action<Point, int> ButtonDown, ButtonUp;
        public event Action<Point> Move;
        public event Action<int> WheelChanged;

        private MouseState _lastState;

        public MouseManager(CoroutineManager manager)
        {
            manager.StartCoroutine(Update());
            _lastState = Mouse.GetState();
        }

        private IEnumerable Update()
        {
            while (true)
            {
                var state = Mouse.GetState();
                if (state.LeftButton != _lastState.LeftButton)
                {
                    if (state.LeftButton == ButtonState.Pressed && ButtonDown != null)
                        ButtonDown(state.Position, 0);
                    else if (state.LeftButton == ButtonState.Released && ButtonDown != null)
                        ButtonUp(state.Position, 0);
                }

                if (state.RightButton != _lastState.RightButton)
                {
                    if (state.RightButton == ButtonState.Pressed && ButtonDown != null)
                        ButtonDown(state.Position, 1);
                    else if (state.RightButton == ButtonState.Released && ButtonDown != null)
                        ButtonUp(state.Position, 1);
                }

                if (state.MiddleButton != _lastState.MiddleButton)
                {
                    if (state.MiddleButton == ButtonState.Pressed && ButtonDown != null)
                        ButtonDown(state.Position, 2);
                    else if (state.MiddleButton == ButtonState.Released && ButtonDown != null)
                        ButtonUp(state.Position, 2);
                }

                if (state.ScrollWheelValue != _lastState.ScrollWheelValue && WheelChanged != null)
                    WheelChanged(state.ScrollWheelValue);

                if (state.Position != _lastState.Position && Move != null)
                    Move(state.Position);

                _lastState = state;
                yield return null;
            }
        }
    }
}
