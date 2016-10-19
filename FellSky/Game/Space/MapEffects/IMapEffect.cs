using Artemis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Space.MapEffects
{
    public interface IMapEffect
    {
        string Name { get; }
        string IconId { get; }
        string Description { get; }

        void ApplyToMap(SpaceMap map, EntityWorld world);
    }
}
