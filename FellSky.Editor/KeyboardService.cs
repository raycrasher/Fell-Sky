using FellSky.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WKeys = System.Windows.Input.Key;
using XKeys = Microsoft.Xna.Framework.Input.Keys;
using System.Windows.Input;
using System.Windows;

namespace FellSky.Editor
{
    class KeyboardService : IKeyboardService
    {
        public event Action<XKeys> KeyDown;
        public event Action<XKeys> KeyUp;

        Dictionary<WKeys, XKeys> _WKeyToXKey = new Dictionary<WKeys, XKeys>();
        Dictionary<XKeys, WKeys> _XKeyToWKey = new Dictionary<XKeys, WKeys>();
        

        private void ConstructKeyMap()
        {
            var wKeys = Enum.GetNames(typeof(WKeys));
            var xKeys = Enum.GetNames(typeof(XKeys));

            var keys = wKeys.Intersect(xKeys).Select(
                n => Tuple.Create((WKeys)Enum.Parse(typeof(WKeys), n), (XKeys)Enum.Parse(typeof(XKeys), n))
                );
            _WKeyToXKey = keys.ToDictionary(k => k.Item1, k => k.Item2);
            _XKeyToWKey = keys.ToDictionary(k => k.Item2, k => k.Item1);
            _WKeyToXKey[WKeys.LeftCtrl] = XKeys.LeftControl;
            _WKeyToXKey[WKeys.RightCtrl] = XKeys.RightControl;
            _XKeyToWKey[XKeys.LeftControl] = WKeys.LeftCtrl;
            _XKeyToWKey[XKeys.RightControl] = WKeys.RightCtrl;
        }

        public KeyboardService(UIElement ctl)
        {
            ctl.KeyDown += OnKeyDown;
            ctl.KeyUp += OnKeyUp;
            ConstructKeyMap();
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            XKeys key;
            if(_WKeyToXKey.TryGetValue(e.Key, out key))
                KeyUp?.Invoke(key);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            XKeys key;
            if (_WKeyToXKey.TryGetValue(e.Key, out key))
                KeyDown?.Invoke(key);
        }

        public bool IsKeyDown(XKeys key)
        {
            return System.Windows.Input.Keyboard.IsKeyDown(_XKeyToWKey[key]);
        }
    }
}
