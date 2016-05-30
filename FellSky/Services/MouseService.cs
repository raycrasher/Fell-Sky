using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;

namespace FellSky.Services
{
    public class MouseService: IMouseService
    {
        public event Action<Point, int> ButtonDown, ButtonUp;
        public event Action<Point> Move;
        public event Action<int> WheelChanged;

        public Vector2 ScreenPosition {
            get { return new Vector2(_position.X, _position.Y); }
            set {
                Mouse.SetPosition((int)value.X,(int)value.Y);
                _position = new Point((int)value.X, (int)value.Y);
            }
        }

        private MouseState _lastState;
        private Point _position;

        public MouseService(CoroutineService coroutineService)
        {
            coroutineService.StartCoroutine(Update());
            _lastState = Mouse.GetState();
            _position = _lastState.Position;
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
                        ButtonDown(state.Position, 2);
                    else if (state.RightButton == ButtonState.Released && ButtonDown != null)
                        ButtonUp(state.Position, 2);
                }

                if (state.MiddleButton != _lastState.MiddleButton)
                {
                    if (state.MiddleButton == ButtonState.Pressed && ButtonDown != null)
                        ButtonDown(state.Position, 1);
                    else if (state.MiddleButton == ButtonState.Released && ButtonDown != null)
                        ButtonUp(state.Position, 1);
                }

                if (state.ScrollWheelValue != _lastState.ScrollWheelValue && WheelChanged != null)
                    WheelChanged(state.ScrollWheelValue - _lastState.ScrollWheelValue);

                if (state.Position != _lastState.Position && Move != null)
                    Move(state.Position);

                _lastState = state;
                _position = state.Position;
                yield return null;
            }
        }
    }
}
