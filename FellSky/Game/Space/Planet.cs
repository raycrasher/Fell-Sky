using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Space
{
    public enum PlanetType
    {
        AirlessRock,
        Terran,
        Desert,
        Ice,
        JovianGasGiant,
        IceGasGiant,
        Volcanic,
        Primordial,
        Jungle,
        
        Sulfur,
        CloudyRock,
        Asteroid,

        Mercury,
        Venus,
        Earth,
        Mars,
        Jupiter,
        Saturn,
        Uranus,
        Neptune,
        Pluto

    }

    public class Planet: SpaceObject
    {
        public OrbitalParameters OrbitalParameters { get; set; } = new OrbitalParameters(10e6f, 0.016f, 102.947, 358.17f); // Earth

        public Planet(string name, string sprite, PlanetType type, float radius, OrbitalParameters orbitalParameters, params SpaceObject[] children)
            : base(children)
        {
            Name = name;
            SpriteId = sprite;
            Type = type;
            OrbitalParameters = orbitalParameters;
        }

        public PlanetType Type { get; set; }
    }
}
