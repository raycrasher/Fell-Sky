using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using FellSky.Services;
using Microsoft.Xna.Framework;
using FellSky.Components;

namespace FellSky.Systems
{
    public class CameraUiDraggingSystem: Artemis.System.TagSystem
    {
        private IMouseService _mouse;
        private bool _mouseDown = false;
        private bool _isDragging = false;
        private Vector2 _offset;
        private Vector2 _origin;

        public CameraUiDraggingSystem(string cameraTag, IMouseService mouse)
            : base(cameraTag)
        {
            _mouse = mouse;
        }

        public string CameraTag { get; private set; }

        public override void LoadContent()
        {
            _mouse.ButtonDown += OnButtonDown;
            _mouse.ButtonUp += OnButtonUp;
            base.LoadContent();
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
            if (_mouseDown && !_isDragging) {
                var camera = entity.GetComponent<CameraComponent>();
                _offset = camera.ScreenToCameraSpace(_mouse.ScreenPosition);
                _isDragging = true;
                var xform = entity.GetComponent<Transform>();
                _origin = xform.Position;
            }
            else if (!_mouseDown && _isDragging) _isDragging = false;

            if (_isDragging)
            {
                var xform = entity.GetComponent<Transform>();
                var camera = entity.GetComponent<CameraComponent>();
                xform.Position = _origin + (_offset - camera.ScreenToCameraSpace(_mouse.ScreenPosition));
            }
        }
    }
}
