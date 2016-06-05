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
    public class ShipComponent: IComponent, IShipEditorEditableComponent
    {
        public Ship Ship { get; set; }

        public IList<PartEntityPair> PartEntities { get; set; } = new List<PartEntityPair>();
        public IReadOnlyList<PartEntityPair> Thrusters { get; set; } = new List<PartEntityPair>();
        public List<Entity> Turrets { get; set; } = new List<Entity>();

        public float AngularTorque { get; set; }
        public Vector2 LinearThrustVector { get; set; }
        public bool AttemptBoost { get; set; }       
        public Vector2 CenterOfMass { get; set; }

        public List<Entity> AdditionalRigidBodyEntities { get; set; } = new List<Entity>();

        public float MaxHeat { get; set; } = 1000;
        public float CurrentHeat { get; set; } = 0;
        public float MaxIonization { get; set; } = 1000;
        public float CurrentIonization { get; set; } = 0;

        IShipEditorEditableModel IShipEditorEditableComponent.Model => Ship;

        // functions

        public ShipComponent(Ship ship)
        {
            Ship = ship;
        }

        
    }
}
