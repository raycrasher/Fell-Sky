using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Campaign.Bases
{
    public class BaseLayoutChanged : EventArgs
    {
        public Base Base { get; private set; }
        public BaseSection Section { get; private set; }
        public BaseFacility Facility { get; private set; }

        public BaseLayoutChanged(Base _base, BaseSection section, BaseFacility facility)
        {
            Base = _base;
            Section = section;
            Facility = facility;
        }
    }

    public abstract class Base
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<BaseSection> Sections { get; set; }

        public event EventHandler<BaseLayoutChanged> BaseLayoutChanged;
    }
}
