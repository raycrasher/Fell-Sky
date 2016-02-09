using System;
using System.Windows.Input;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;
using FellSky.Services;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace FellSky.Editor
{
    class MouseService : IMouseService
    {
        private System.Windows.FrameworkElement host;
        private Vector2 _newScreenPos;

        public MouseService(System.Windows.FrameworkElement host)
        {
            this.host = host;
            var pos = Mouse.GetPosition(host);
            _newScreenPos = new Vector2((float)pos.X, (float)pos.Y);
            _screenPos = _newScreenPos;
            host.MouseMove += OnMouseMove;
            host.MouseDown += OnButtonDown;
            host.MouseUp += OnButtonUp;
            host.MouseWheel += OnWheel;
        }

        private void OnWheel(object sender, MouseWheelEventArgs e)
        {
            _updateActions.Add(() => WheelChanged?.Invoke(e.Delta));
        }

        private void OnButtonUp(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(host);
            _newScreenPos = new Vector2((float)pos.X, (float)pos.Y);
            _updateActions.Add(()=>ButtonUp?.Invoke(new Point((int)pos.X, (int)pos.Y), (int) e.ChangedButton));
        }

        private void OnButtonDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(host);
            _newScreenPos = new Vector2((float)pos.X, (float)pos.Y);
            _updateActions.Add(()=>ButtonDown?.Invoke(new Point((int)pos.X, (int)pos.Y), (int)e.ChangedButton));

        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(host);
            _newScreenPos = new Vector2((float)pos.X, (float)pos.Y);
            _updateActions.Add(() => Move?.Invoke(new Point((int)pos.X, (int)pos.Y)));
        }

        public Vector2 ScreenPosition
        {
            get { return _screenPos; }
            set {

                _updateActions.Add(() =>
                {
                    NativeMethods.SetCursorPos((int)value.X, (int)value.Y);
                    _newScreenPos = value;
                });
            }
        }

        public event Action<Point, int> ButtonDown;
        public event Action<Point, int> ButtonUp;
        public event Action<Point> Move;
        public event Action<int> WheelChanged;

        private readonly ConcurrentBag<Action> _updateActions = new ConcurrentBag<Action>();
        private Vector2 _screenPos;

        static class NativeMethods
        {
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool SetCursorPos(int X, int Y);
        }

        internal void Update()
        {
            _screenPos = _newScreenPos;
            Action action = null;
            while(_updateActions.TryTake(out action))
            {
                action?.Invoke();
            }
        }
    }
}
