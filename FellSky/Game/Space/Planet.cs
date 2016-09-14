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
        public OrbitalParameters OrbitalParameters { get; set; } = new OrbitalParameters(10e6f, 0.016f, 102.947, 358.17f); // Earth

        public Planet(string name, string sprite, PlanetType type, float radius, OrbitalParameters orbitalParameters, params SpaceObject[] children)
            : base(children)
        {
            Name = name;
            TextureId = sprite;
            Type = type;
            OrbitalParameters = orbitalParameters;
            Radius = radius;
        }

        public override Entity CreateEntity(EntityWorld world)
        {
            var entity = base.CreateEntity(world);
            var xform = entity.GetComponent<Transform>();
            xform.Scale *= (float)Math.Log(Radius) * 1e-1f;
            var pos = OrbitalParameters.GetPositionAtTime(DateTime.Today);
            //pos.X = (float)Math.Log(Math.Abs(pos.X)) * Math.Sign(pos.X);
            //pos.Y = (float)Math.Log(Math.Abs(pos.Y)) * Math.Sign(pos.Y);

            //if (float.IsNaN(pos.X)) pos.X = 0;
            //if (float.IsNaN(pos.Y)) pos.Y = 0;

            xform.Position = pos * 1e-4f;
            return entity;
        }

        public PlanetType Type { get; set; }
        public float Radius { get; set; }
    }
}
