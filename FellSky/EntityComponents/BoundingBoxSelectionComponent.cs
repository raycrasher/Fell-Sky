﻿using Artemis.Interface;
using Microsoft.Xna.Framework;
using System;

namespace FellSky.EntityComponents
{
    public class BoundingBoxSelectionComponent: IComponent
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

        public BoundingBoxSelectionComponent() { }
        public BoundingBoxSelectionComponent(FloatRect boundingBox)
        {
            BoundingBox = boundingBox;
        }

        public bool IsEnabled { get; set; } = true;
    }
}