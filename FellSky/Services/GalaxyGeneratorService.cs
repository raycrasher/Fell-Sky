using FellSky.Game.Space;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Services
{
    public class GalaxyGeneratorService
    {
        Random _rng = new Random();
        public virtual Galaxy CreateGalaxy(int initialStars = 50)
        {            
            var galaxy = new Galaxy();

            galaxy.StarSystems.Add(CreateSolSystem());

            return galaxy;
        }

        public virtual SpaceObject CreateSolSystem()
        {
            var sol = new Star(
                "Sol", "sun", StellarClass.G, 2, 695700, new Vector2(200,200),

                new Planet("Mercury", "mercury", PlanetType.AirlessRock, 2440, new OrbitalParameters(57.909e6, 0.205, 77.445, 174.796f.DegreeToRadian() )),
                new Planet("Venus", "venus", PlanetType.CloudyRock, 6052, new OrbitalParameters(108.208e6f, 0.00672d, 131.564f, 50.115f.DegreeToRadian() )),
                new Planet("Earth", "earth", PlanetType.Earth, 6371, new OrbitalParameters(10e6d, 0.016d, 102.947, 358.17f.DegreeToRadian()),
                    new Planet("Luna", "moon", PlanetType.AirlessRock, 1737, new OrbitalParameters(384.399e3, 0.0549, 0, 0))
                ) /*,
                new Planet("Mars", "mars", PlanetType.Desert, 3000, new OrbitalParameters(),
                    new Planet("Phobos", "phobos", PlanetType.Asteroid, 500, new OrbitalParameters()),
                    new Planet("Deimos", "deimos", PlanetType.Asteroid, 500, new OrbitalParameters())
                ),
                //new AsteroidBelt("Asteroid Belt", ...)
                new Planet("Ceres", "rockplanet", PlanetType.AirlessRock, 3000, new OrbitalParameters()),
                new Planet("Jupiter", "jupiter", PlanetType.Jupiter, 1000, new OrbitalParameters(),
                    new Planet("Europa", "tundraplanet", PlanetType.Jupiter, 1000, new OrbitalParameters()),
                    new Planet("Ganymede", "primordialplanet", PlanetType.Jupiter, 1000, new OrbitalParameters()),
                    new Planet("Io", "sulfurplanet", PlanetType.Jupiter, 1000, new OrbitalParameters()),
                    new Planet("Callisto", "crystalplanet", PlanetType.Jupiter, 1000, new OrbitalParameters())
                )*/
            );

            return sol;
        }
    }
}
