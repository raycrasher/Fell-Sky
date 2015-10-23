using Artemis.Interface;
using FarseerPhysics.Dynamics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.EntityComponents
{
    public class PhysicsBodyComponent: IComponent
    {
        public Body Body { get; set; }
    }
}
