using Artemis.Interface;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public class PlayerControlsComponent : IComponent
    {
        public Keys UpKey { get; set; } = Keys.W;
        public Keys DownKey { get; set; } = Keys.S;
        public Keys LeftKey { get; set; } = Keys.A;
        public Keys RightKey { get; set; } = Keys.D;
        public Keys AltLeftKey { get; set; } = Keys.Q;
        public Keys AltRightKey { get; set; } = Keys.E;
        public Keys BoostKey { get; set; } = Keys.Space;
        public Keys ToggleOrthogonalMovementKey { get; set; } = Keys.V;

        public bool MouseHeadingControl { get; set; } = false;
        public bool OrthogonalMovement { get; set; } = false;
    }
}
