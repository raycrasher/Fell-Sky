using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Mechanics.Ships
{
    public enum PowerStatus
    {
        Unpowered, Off, On, Cut, Low, PowerSave, Overdrive, Overload
    }

    public enum HealthStatus
    {
        Full, Damaged, Critical, Disabled, Destroyed
    }

    public interface IShipSubsystem
    {
  
    }
}
