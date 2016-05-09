using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using FellSky.Services;
using Microsoft.Xna.Framework;
using FellSky.Components;
using Microsoft.Xna.Framework.Input;

namespace FellSky.Systems
{
    public class CameraControlSystem: Artemis.System.TagSystem
    {
        private IMouseService _mouse;
        private IKeyboardService _keyboard;
        private bool _mouseDown = false;
        private bool _isDragging = false;
        private Vector2 _offset;
        private Vector2 _origin;
        private float _targetZoom = 1;
        private float _currentZoom = 1;
        private float _zoomLerpTime=1;

        public float MinZoom { get; set; } = 0;
        public float MaxZoom { get; set; } = 3;

        //private ITimerService _timer;

        public CameraControlSystem()
            : base(Constants.ActiveCameraTag)
        {
            _mouse = ServiceLocator.Instance.GetService<IMouseService>();
            _keyboard = ServiceLocator.Instance.GetService<IKeyboardService>();
            //_timer = timer;
        }

        public string CameraTag { get; private set; }

        public override void LoadContent()
        {
            _mouse.ButtonDown += OnButtonDown;
            _mouse.ButtonUp += OnButtonUp;
            _mouse.WheelChanged += WheelChanged;
            base.LoadContent();
        }

        private void WheelChanged(int delta)
        {
            if (_keyboard.IsKeyDown(Keys.LeftShift) ||
                _keyboard.IsKeyDown(Keys.LeftAlt) ||
                _keyboard.IsKeyDown(Keys.LeftControl) ||
                _keyboard.IsKeyDown(Keys.RightShift) ||
                _keyboard.IsKeyDown(Keys.RightAlt) ||
                _keyboard.IsKeyDown(Keys.RightControl)) return;
            if (delta < 0) _targetZoom -= 0.1f;
            else if (delta > 0) _targetZoom += 1.1f;
            _zoomLerpTime = 0;
            _targetZoom = MathHelper.Clamp(_targetZoom, MinZoom, MaxZoom);
        }

        private void OnButtonUp(Point pos, int button)
        {
            if (button == 1) _mouseDown = false;
        }

        private void OnButtonDown(Point pos, int button)
        {
            if(button == 1) _mouseDown = true;
        }

        public override void Process(Entity entity)
        {
            if (_mouseDown && !_isDragging)
            {
                var camera = entity.GetComponent<Camera>();
                _offset = _mouse.ScreenPosition;
                _isDragging = true;
                var xform = entity.GetComponent<Transform>();
                _origin = xform.Position;
            }
            else if (!_mouseDown && _isDragging)
            {
                _isDragging = false;
            }

            if (_isDragging)
            {
                var xform = entity.GetComponent<Transform>();
                var camera = entity.GetComponent<Camera>();
                xform.Position = _origin + (_offset - _mouse.ScreenPosition);
            }

            // if zooming
            if(_zoomLerpTime < 1)
            {
                _currentZoom = MathHelper.Lerp(_currentZoom, _targetZoom, _zoomLerpTime);
                _zoomLerpTime += 0.1f;
                var camera = entity.GetComponent<Camera>();
                camera.Zoom = _currentZoom;
            }
        }
    }
}
