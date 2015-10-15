using Artemis.Interface;
using FellSky.Mechanics.Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.EntityComponents
{
    public enum WarpDriveState
    {
        Idle, Charging, Warping, Dewarping, 
    }
    public class WarpDriveComponent: IComponent
    {
        public WarpDrive WarpDrive { get; set; }
        public float ExoticMatterAmount { get; set; }
    }
}
