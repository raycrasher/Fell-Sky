using Artemis.Interface;
using FarseerPhysics.Dynamics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace FellSky.Components
{
    public class RigidBodyComponent: IComponent
    {
        public Body Body { get; set; }

        public float Rotation {
            get { return Body.Rotation; }
            set { Body.Rotation = value; }
        }
        public Vector2 WorldPosition {
            get { return Body.Position / Constants.PhysicsUnitScale; }
            set { Body.Position = value * Constants.PhysicsUnitScale; }
        }
    }
}
