using System;
using Artemis;
using Artemis.Interface;
using FellSky.Models.Ships.Parts;
using FellSky.Services;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace FellSky.Components
{
    public class HullComponent: ShipPartComponent<Hull>
    {
        public HullComponent(Hull hull, Entity ship)
            : base(hull, ship)
        {
        }
    }
}