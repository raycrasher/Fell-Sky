using System;
using System.Windows.Input;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;
using FellSky.Services;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;

namespace FellSky.Editor
{
    class MouseService : IMouseService
    {
        private System.Windows.FrameworkElement host;

        MouseButtonState _left, _right, _middle;

        public MouseService(System.Windows.FrameworkElement host)
        {
            this.host = host;
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
            var pos = ScreenPosition;
            _updateActions.Add(()=>ButtonUp?.Invoke(new Point((int)pos.X, (int)pos.Y), (int) e.ChangedButton));
            _left = e.LeftButton;
            _right = e.RightButton;
            _middle = e.MiddleButton;
        }

        private void OnButtonDown(object sender, MouseButtonEventArgs e)
        {
            var pos = ScreenPosition;
            _updateActions.Add(()=>ButtonDown?.Invoke(new Point((int)pos.X, (int)pos.Y), (int)e.ChangedButton));
            _left = e.LeftButton;
            _right = e.RightButton;
            _middle = e.MiddleButton;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var pos = ScreenPosition;
            _updateActions.Add(() => Move?.Invoke(new Point((int)pos.X, (int)pos.Y)));
        }

        public Vector2 ScreenPosition
        {
            get {
                NativeMethods.POINT pt;
                NativeMethods.GetCursorPos(out pt);
                var vec = host.PointFromScreen(new System.Windows.Point(pt.X,pt.Y));
                return new Vector2((float)vec.X, (float)vec.Y);
            }
            set {
                var vec = value;//host.PointToScreen(new System.Windows.Point(value.X, value.Y));
                NativeMethods.SetCursorPos((int)vec.X, (int)vec.Y);
            }
        }

        public event Action<Point, int> ButtonDown;
        public event Action<Point, int> ButtonUp;
        public event Action<Point> Move;
        public event Action<int> WheelChanged;

        private ConcurrentBag<Action> _updateActions = new ConcurrentBag<Action>();

        static class NativeMethods
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct POINT
            {
                public int X;
                public int Y;

                public POINT(int x, int y)
                {
                    this.X = x;
                    this.Y = y;
                }

                public POINT(System.Drawing.Point pt) : this(pt.X, pt.Y) { }

                public static implicit operator System.Drawing.Point(POINT p)
                {
                    return new System.Drawing.Point(p.X, p.Y);
                }

                public static implicit operator POINT(System.Drawing.Point p)
                {
                    return new POINT(p.X, p.Y);
                }
            }

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool SetCursorPos(int X, int Y);

            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool GetCursorPos(out POINT lpPoint);
        }

        internal void Update()
        {
            //_screenPos = _newScreenPos;
            Action action = null;
            while(_updateActions.TryTake(out action))
            {
                action?.Invoke();
            }
        }

        public bool IsMouseDown(int button)
        {
            switch (button)
            {
                case 0:
                    return _left == MouseButtonState.Pressed;
                case 1:
                    return _right == MouseButtonState.Pressed;
                case 2:
                    return _middle == MouseButtonState.Pressed;
                default:
                    return false;
            }
        }
    }
}
