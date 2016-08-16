using Artemis;
using Artemis.Interface;
using FellSky.Game.Ships.Parts;
using FellSky.Game.Ships;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using FellSky.Services;

namespace FellSky.Components
{
    public class ShipComponent: IComponent
    {
        public Ship Ship { get; set; }

        public List<Entity> Thrusters { get; set; } = new List<Entity>();
        public List<Entity> Weapons { get; set; } = new List<Entity>();
        public List<Entity> Hardpoints { get; set; } = new List<Entity>();

        public float AngularTorque { get; set; }
        public Vector2 LinearThrustVector { get; set; }
        public bool AttemptBoost { get; set; }
        public Vector2 CenterOfMass { get; set; }

        public ShipVariant Variant { get; set; }
        
        // functions

        public ShipComponent(Ship ship)
        {
            Ship = ship;
        }

        
    }
}
