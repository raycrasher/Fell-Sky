using Artemis.Interface;
using Microsoft.Xna.Framework;
using System;

namespace FellSky.Components
{
    public class BoundingBoxSelectorComponent: IComponent
    {
        public FloatRect BoundingBox { get; set; }
        public bool IsSelected {
            get { return _isSelected; }
            set {
                if(value!=_isSelected)
                {
                    _isSelected = value;
                    SelectedChanged?.Invoke(this, null);
                    return;
                }
            }
        }
        public float Parallax { get; set; } = 1;
        public event EventHandler SelectedChanged;

        private bool _isSelected = false;

        public BoundingBoxSelectorComponent() { }
        public BoundingBoxSelectorComponent(FloatRect boundingBox)
        {
            BoundingBox = boundingBox;
        }

        public bool IsEnabled { get; set; } = true;
    }
}
