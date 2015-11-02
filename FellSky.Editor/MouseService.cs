using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Xna.Framework;

namespace FellSky.Editor
{
    class MouseService : Framework.IMouseService
    {
        private System.Windows.FrameworkElement host;

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
            WheelChanged?.Invoke(e.Delta);
        }

        private void OnButtonUp(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(host);
            ButtonUp?.Invoke(new Point((int)pos.X, (int)pos.Y), (int) e.ChangedButton);
        }

        private void OnButtonDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(host);
            ButtonDown?.Invoke(new Point((int)pos.X, (int)pos.Y), (int)e.ChangedButton);

        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(host);
            ScreenPosition = new Vector2((float)pos.X, (float)pos.Y);
            Move?.Invoke(new Point((int)pos.X, (int)pos.Y));
        }

        public Vector2 ScreenPosition { get; private set; }

        public event Action<Point, int> ButtonDown;
        public event Action<Point, int> ButtonUp;
        public event Action<Point> Move;
        public event Action<int> WheelChanged;
    }
}
