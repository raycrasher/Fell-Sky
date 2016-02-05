﻿using Artemis.Interface;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public class RigidBodyFixtureComponent: IComponent
    {
        public RigidBodyFixtureComponent(Fixture fixture)
        {
            Fixture = fixture;
        }


        public Fixture Fixture { get; set; }
    }
}