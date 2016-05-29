using Artemis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public class MouseHoverComponent: IComponent
    {
        private bool _isHover;

        public bool IsHover {
            get { return _isHover; }
            set {
                if (value != _isHover)
                {
                    _isHover = value;
                    HoverChanged?.Invoke(this,null);
                }
            }
        }
        public event EventHandler HoverChanged;
    }
}
