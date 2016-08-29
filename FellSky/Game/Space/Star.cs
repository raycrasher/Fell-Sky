using Artemis.Interface;
using FellSky.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using FellSky.Services;

namespace FellSky.Game.Space
{
    public enum StellarClass
    {
        O,
        B,
        A,
        F,
        G,
        K,
        M,
        // dwarfs
        L,
        T,

        W, // wolf-rayet - giant space laser stars
        C, // carbon star

        E, // exotic
        X, // black hole
        N, // neutron star
    }

    public class StarClass
    {
        public string Name;
        public double Mass;
        public double Luminosity;
        public double Radius;
        public double Temperature;
        public float ColorIndex;
        public float AbsoluteMagnitude;
        public float BolometricCorrection;
        public float BolometricMagnitude;
        public Color Color;
    }

    public class Star: SpaceObject
    {

        public Star(string name, string sprite, StellarClass stellarClass, float temperature, float radius, Vector2 position, params SpaceObject[] children)
            :base(children)
        {
            Name = name;
            Class = stellarClass;
            Temperature = temperature;
            PhotosphereRadius = radius;
            CoronaRadius = radius * 1.25f;
            SpriteId = sprite;
        }

        public StellarClass Class { get; set; }
        public Color Color { get; set; }
        public float Temperature { get; set; }
        public float CoronaRadius { get; set; }
        public float PhotosphereRadius { get; set; }

        public const string StellarClassDefinitionFile = "StarTypes.csv";

        public readonly static Dictionary<string, StarClass> StellarDefinitions = new Dictionary<string, StarClass>();

        public static void LoadStellarClasses()
        {
            using(var stream = new System.IO.StreamReader(StellarClassDefinitionFile))
            {
                var csvReader = new CsvHelper.CsvReader(stream);
                // skip 2 header rows
                csvReader.ReadHeader();
                csvReader.Read();
                while (csvReader.Read())
                {
                    var item = new StarClass
                    {
                        Name = csvReader.GetField(0),
                        Mass = csvReader.GetField<double>(1),
                        Luminosity = csvReader.GetField<double>(2),
                        Radius = csvReader.GetField<double>(3),
                        Temperature = csvReader.GetField<double>(4),
                        ColorIndex = csvReader.GetField<float>(5),
                        AbsoluteMagnitude = csvReader.GetField<float>(6),
                        BolometricCorrection = csvReader.GetField<float>(7),
                        BolometricMagnitude = csvReader.GetField<float>(8),
                    };
                    var rgb = csvReader.GetField(9).Split(' ').Select(s => byte.Parse(s)).ToArray();
                    item.Color = new Color(rgb[0], rgb[1], rgb[2]);
                    StellarDefinitions[item.Name] = item;
                }
            }
        }

        public override Entity CreateEntity(EntityWorld world)
        {
            var entity = base.CreateEntity(world);
            var xform = entity.GetComponent<Transform>();
            xform.Scale *= PhotosphereRadius * 1e-5f;
            return entity;
        }
    }
}
