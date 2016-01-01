using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;

namespace FellSky.Editor
{
    class MouseService : Framework.IMouseService
    {
        private System.Windows.FrameworkElement host;
        private Vector2 _screenPosition;

        public MouseService(System.Windows.FrameworkElement host)
        {
            this.host = host;
            var pos = Mouse.GetPosition(host);
            _screenPosition = new Vector2((float)pos.X, (float)pos.Y);
            host.MouseMove += OnMouseMove;
            host.MouseDown += OnButtonDown;
            host.MouseUp += OnButtonUp;
            host.MouseWheel += OnWheel;
        }

        private void OnWheel(object sender, MouseWheelEventArgs e)
        {
            WheelChanged?.Invoke(e.Delta);
        }

        private void OnButtonUp(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(host);
            _screenPosition = new Vector2((float)pos.X, (float)pos.Y);
            ButtonUp?.Invoke(new Point((int)pos.X, (int)pos.Y), (int) e.ChangedButton);
        }

        private void OnButtonDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(host);
            _screenPosition = new Vector2((float)pos.X, (float)pos.Y);
            ButtonDown?.Invoke(new Point((int)pos.X, (int)pos.Y), (int)e.ChangedButton);

        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(host);
            _screenPosition = new Vector2((float)pos.X, (float)pos.Y);
            Move?.Invoke(new Point((int)pos.X, (int)pos.Y));
        }

        public Vector2 ScreenPosition
        {
            get { return _screenPosition; }
            set {
                NativeMethods.SetCursorPos((int)value.X, (int)value.Y);
                _screenPosition = value;
            }
        }

        public event Action<Point, int> ButtonDown;
        public event Action<Point, int> ButtonUp;
        public event Action<Point> Move;
        public event Action<int> WheelChanged;

        static class NativeMethods
        {
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool SetCursorPos(int X, int Y);
        }
    }
}
