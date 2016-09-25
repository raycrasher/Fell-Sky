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
        private float _moveLerpTime = 1;
        private Vector2 _targetPosition, _lastMovePosition;
        private float _lastZoom;
        private ITimerService _timer;
        private Camera _camera;

        public float MinZoom { get; set; } = 0.5f;
        public float MaxZoom { get; set; } = 5;

        //private ITimerService _timer;

        public CameraControlSystem()
            : base(Constants.ActiveCameraTag)
        {
            _mouse = ServiceLocator.Instance.GetService<IMouseService>();
            _keyboard = ServiceLocator.Instance.GetService<IKeyboardService>();
            //_timer = timer;
        }

        public override void LoadContent()
        {
            _mouse.ButtonDown += OnButtonDown;
            _mouse.ButtonUp += OnButtonUp;
            _mouse.WheelChanged += WheelChanged;
            _timer = ServiceLocator.Instance.GetService<ITimerService>();
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
            if (delta > 0)
            {
                _lastZoom = _currentZoom;
                _targetZoom -= 0.2f * _targetZoom > 1 ? (_currentZoom * 0.5f) : 1;
            }
            else if (delta < 0)
            {
                _lastZoom = _currentZoom;
                _targetZoom += 0.2f * _targetZoom > 1 ? (_currentZoom + 1) : 1;
            }
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
            _camera = entity.GetComponent<Camera>();

            if (_mouseDown && !_isDragging)
            {
                
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
                
                xform.Position = _origin + (_offset - _mouse.ScreenPosition) * (_currentZoom > 1 ? (float)Math.Log(_currentZoom)* (float)Math.Log(_currentZoom) : 1f);
            }

            // if zooming
            if(_zoomLerpTime < 1)
            {
                _currentZoom = MathHelper.SmoothStep(_lastZoom, _targetZoom, _zoomLerpTime);
                _zoomLerpTime += (float)_timer?.DeltaTime.TotalSeconds * 2;
                _camera.Zoom = _currentZoom;
            }

            if(_moveLerpTime < 1)
            {
                _moveLerpTime += (float)_timer.DeltaTime.TotalSeconds * 2;
                _camera.Transform.Position = Vector2.SmoothStep(_lastMovePosition, _targetPosition, _moveLerpTime);
            }
        }

        public void MoveTo(Vector2 position)
        {
            _moveLerpTime = 0;
            _lastMovePosition = _camera?.Transform.Position ?? Vector2.Zero;
            _targetPosition = position;
        }

        public void ZoomTo(float zoom)
        {
            _zoomLerpTime = 0;
            _lastZoom = _currentZoom;
            _targetZoom = MathHelper.Clamp(zoom, MinZoom, MaxZoom);
        }
    }
}
