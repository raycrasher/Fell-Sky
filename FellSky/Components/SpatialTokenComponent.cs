using Artemis;
using Artemis.Interface;
using SharpSteer2.Database;

namespace FellSky.Components
{
    public class SpatialTokenComponent: IComponent
    {
        public ITokenForProximityDatabase<Entity> Token;
    }
}
