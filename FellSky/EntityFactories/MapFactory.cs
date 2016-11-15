using Artemis;
using FellSky.Game.Space;
using FellSky.Game.Space.MapEffects;
using FellSky.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky
{
    public static class MapFactory
    {
        public static Entity CreateBackground(this EntityWorld world, int seed)
        {
            return ServiceLocator.Instance.GetService<Services.SpaceBackgroundGeneratorService>().GenerateBackground(world, seed);
        }

        public static Entity CreateBackground(this EntityWorld world, float starDensity, params NebulaParameters[] nebulae)
        {
            return ServiceLocator.Instance.GetService<Services.SpaceBackgroundGeneratorService>().GenerateBackground(world, starDensity, nebulae);
        }

        public static void AddBackgroundPlanet(this EntityWorld world, PlanetType planetType)
        {
            
        }

        public static void CreateMap(this EntityWorld world, string map_id, string name, params IMapEffect[] effects)
        {

        }

        private static void DesertPlanetOrbit(EntityWorld world)
        {
            world.CreateBackground(1234);
            world.AddBackgroundPlanet(PlanetType.Desert);
        }
    }
}
