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
    public class ShipComponent: IComponent, IShipPartCollectionComponent
    {
        public Ship Ship { get; set; }

        public List<Entity> Thrusters { get; set; } = new List<Entity>();
        public List<Entity> Turrets { get; set; } = new List<Entity>();

        public float AngularTorque { get; set; }
        public Vector2 LinearThrustVector { get; set; }
        public bool AttemptBoost { get; set; }       
        public Vector2 CenterOfMass { get; set; }

        public float MaxHeat { get; set; } = 1000;
        public float CurrentHeat { get; set; } = 0;
        public float MaxIonization { get; set; } = 1000;
        public float CurrentIonization { get; set; } = 0;

        public Color BaseDecalColor { get; set; }
        public Color TrimDecalColor { get; set; }

        IShipPartCollection  IShipPartCollectionComponent.Model => Ship;

        public ShipVariant Variant { get; set; }
        public List<Entity> Weapons { get; set; } = new List<Entity>();
        public Dictionary<Hardpoint, Entity> Hardpoints { get; set; } = new Dictionary<Hardpoint, Entity>();

        public Entity GetInstalledEntityForHardpoint(Hardpoint hardpoint) 
            => Hardpoints[hardpoint]?.GetComponent<HardpointComponent>().InstalledEntity;

        // functions

        public ShipComponent(Ship ship)
        {
            Ship = ship;
        }

        
    }
}
