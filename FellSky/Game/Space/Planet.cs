using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;

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
        public Planet(string name, string sprite, PlanetType type, float radius, float orbitRadius=1, float orbitPeriod=365, params SpaceObject[] children)
            : base(children)
        {
            Name = name;
            TextureId = sprite;
            Type = type;
            OrbitRadius = orbitRadius;
            OrbitPeriod = orbitPeriod;
            Radius = radius;
        }

        public override Entity CreateEntity(EntityWorld world)
        {
            var entity = base.CreateEntity(world);
            var xform = entity.GetComponent<Transform>();
            xform.Scale *= (float)Math.Log(Radius) * 1e-1f;
            var pos = GetPositionAtTime(DateTime.Today);

            xform.Position = pos * 500;
            return entity;
        }

        public PlanetType Type { get; set; }
        public float Radius { get; set; }
    }
}
