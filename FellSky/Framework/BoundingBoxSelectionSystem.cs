using System.Collections.Generic;
using Artemis;
using FellSky.Framework;
using FellSky.Graphics;
using Microsoft.Xna.Framework;

namespace FellSky.Framework
{
    [Artemis.Attributes.ArtemisEntitySystem(ExecutionType = Artemis.Manager.ExecutionType.Synchronous, GameLoopType = Artemis.Manager.GameLoopType.Update )]
    public class BoundingBoxSelectionSystem : Artemis.System.EntitySystem
    {
        private IMouseService _mouse;
        private bool _isClick;
        private Vector2 _mousePos;

        public List<Entity> SelectedEntities { get; } = new List<Entity>();

        public int SelectionButton { get; set; } = 0;

        public BoundingBoxSelectionSystem()
            : base(Aspect.All(typeof(Transform), typeof(BoundingBoxSelector)))
        { }

        public override void LoadContent()
        {
            base.LoadContent();
            _mouse = BlackBoard.GetService<IMouseService>();
            _mouse.ButtonDown += OnClick;
        }

        private void OnClick(Point point, int button)
        {
            if (button != SelectionButton) return;
            _isClick = true;
            _mousePos = _mouse.ScreenPosition;
        }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            if (_isClick)
            {
                var camera = BlackBoard.GetEntry<Camera2D>(Camera2D.PlayerCameraName);
                foreach (var entity in entities.Values)
                {
                    var box = entity.GetComponent<BoundingBoxSelector>();
                    if (!box.IsEnabled) continue;

                    var matrix = Matrix.Invert(camera.GetViewMatrix(box.Parallax)) * Matrix.Invert(entity.GetWorldMatrix());
                    
                    Vector2 position = _mousePos;
                    Vector2.Transform(ref position, ref matrix, out position);
                    if (box.BoundingBox.Contains(position))
                    {
                        box.IsSelected = true;
                        SelectedEntities.Add(entity);
                    }
                    else
                    {
                        box.IsSelected = false;
                        SelectedEntities.Remove(entity);
                    }
                }
            }
            _isClick = false;
        }       
    }
}
