using FellSky.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace FellSky.Mechanics.Ships
{
    /// <summary>
    /// The physical parts of a ship. Determines the graphics and collision data of the ship.
    /// </summary>
    public class Hull
    {
        public Thruster[] Thrusters { get; set; }
    }

    public class HullDefinition
    {
        public FarseerPhysics.Collision.Shapes.Shape[] Shapes { get; set; }
        public SpriteGroup
    }
}
