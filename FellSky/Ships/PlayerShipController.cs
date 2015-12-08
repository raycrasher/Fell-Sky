using Artemis.Interface;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Ships
{
    public class PlayerShipController: IComponent
    {
        public Keys UpKey { get; set; }
        public Keys DownKey { get; set; }
        public Keys LeftKey { get; set; }
        public Keys RightKey { get; set; }
        public Keys AltLeftKey { get; set; }
        public Keys AltRightKey { get; set; }
        public Keys BoostKey { get; set; }

        public bool MouseHeadingControl { get; set; } = false;
        public bool OrthogonalMovement { get; set; } = false;
    }
}
