using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Models.Ships.Parts
{
    public enum WarpDriveState
    {
        Idle, Charging, Warping, Dewarping,
    }

    public class WarpDrive: ShipPart
    {
        public float WarpMultiplier { get; set; }
        public float MaxExoticMatterAmount { get; set; }
        public float CurrentExoticMatterAmount { get; set; }
        public float EnergyConsumedPerExoticMatterUnit { get; set; }
        public float HeatProducedPerExoticMatterUnit { get; set; }
        
        public WarpDriveState State { get; set; }

    }
}
