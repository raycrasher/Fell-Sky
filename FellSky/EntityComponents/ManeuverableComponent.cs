using Artemis.Interface;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.EntityComponents
{
    public class ManeuverableComponent: IComponent
    {
        public bool AttemptBoost { get; internal set; }
        public Vector2 LinearThrustVector { get; set; }

        /// <summary>
        /// Torque value between -1 and 1. -1 = max torque going CW, and 1 is max torque going CCW. 0  is no torque.
        /// </summary>
        public float Torque{ get; set; }


    }
}
