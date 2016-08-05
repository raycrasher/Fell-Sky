using Artemis.Interface;
using FellSky.Game.Ships.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FellSky.Game.Ships.Parts;
using Artemis;

namespace FellSky.Components
{
    public class ModuleComponent: IComponent
    {
        public HashSet<IModuleUpgrade> InstalledMods { get; set; } = new HashSet<IModuleUpgrade>();
        public Module Module;
        public Entity HardpointEntity;
    }
}
